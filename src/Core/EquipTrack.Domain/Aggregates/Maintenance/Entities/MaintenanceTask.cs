using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a maintenance task in the CMMS system.
/// Follows DDD principles with proper encapsulation and business logic.
/// </summary>
public sealed class MaintenanceTask : BaseEntity, IAggregateRoot
{
    // Immutable properties (set via constructor or init)
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public MaintenanceTaskType Type { get; init; }
    public MaintenanceTaskPriority Priority { get; private set; }
    
    // Foreign Keys with Ref suffix (following NexusCore convention)
    public Guid AssetRef { get; init; }
    public Guid? AssignedTechnicianRef { get; private set; }
    public Guid CreatedByRef { get; init; }
    
    // Mutable state (controlled through domain methods)
    public MaintenanceTaskStatus Status { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime? StartedDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public decimal EstimatedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public decimal EstimatedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public string? CompletionNotes { get; private set; }
    
    // Navigation properties
    [ForeignKey(nameof(AssetRef))]
    public Asset Asset { get; init; } = null!;
    
    [ForeignKey(nameof(AssignedTechnicianRef))]
    public User? AssignedTechnician { get; private set; }
    
    [ForeignKey(nameof(CreatedByRef))]
    public new User CreatedBy { get; init; } = null!;
    
    // Collections
    private readonly List<MaintenanceTaskSparePart> _spareParts = new();
    public IReadOnlyCollection<MaintenanceTaskSparePart> SpareParts => _spareParts.AsReadOnly();
    
    // EF Core constructor
    private MaintenanceTask() { }
    
    // Private constructor for factory method
    private MaintenanceTask(
        string title,
        string description,
        MaintenanceTaskType type,
        MaintenanceTaskPriority priority,
        Guid assetRef,
        Guid createdByRef,
        DateTime scheduledDate,
        decimal estimatedHours,
        decimal estimatedCost)
    {
        Title = Ensure.NotEmpty(title, nameof(title));
        Description = Ensure.NotEmpty(description, nameof(description));
        Type = type;
        Priority = priority;
        AssetRef = Ensure.NotEmpty(assetRef, nameof(assetRef));
        CreatedByRef = Ensure.NotEmpty(createdByRef, nameof(createdByRef));
        ScheduledDate = Ensure.NotDefault(scheduledDate, nameof(scheduledDate));
        EstimatedHours = Ensure.NotNegative(estimatedHours, nameof(estimatedHours));
        EstimatedCost = Ensure.NotNegative(estimatedCost, nameof(estimatedCost));
        Status = MaintenanceTaskStatus.Scheduled;
    }
    
    /// <summary>
    /// Factory method to create a new maintenance task.
    /// </summary>
    public static MaintenanceTask Create(
        string title,
        string description,
        MaintenanceTaskType type,
        MaintenanceTaskPriority priority,
        Guid assetRef,
        Guid createdByRef,
        DateTime scheduledDate,
        decimal estimatedHours,
        decimal estimatedCost)
    {
        return new MaintenanceTask(
            title,
            description,
            type,
            priority,
            assetRef,
            createdByRef,
            scheduledDate,
            estimatedHours,
            estimatedCost);
    }
    
    #region Domain Behaviors
    
    /// <summary>
    /// Assigns a technician to this maintenance task.
    /// </summary>
    public Result AssignTechnician(Guid technicianRef)
    {
        if (Status is MaintenanceTaskStatus.Completed or MaintenanceTaskStatus.Cancelled)
            return Result.Error("Cannot assign technician to completed or cancelled task");
        
        AssignedTechnicianRef = Ensure.NotEmpty(technicianRef, nameof(technicianRef));
        
        if (Status == MaintenanceTaskStatus.Scheduled)
            Status = MaintenanceTaskStatus.Assigned;
        
        return Result.Success();
    }
    
    /// <summary>
    /// Starts the maintenance task.
    /// </summary>
    public Result Start()
    {
        if (Status != MaintenanceTaskStatus.Assigned)
            return Result.Error($"Cannot start task with status: {Status}");
        
        if (!AssignedTechnicianRef.HasValue)
            return Result.Error("Cannot start task without assigned technician");
        
        StartedDate = DateTime.UtcNow;
        Status = MaintenanceTaskStatus.InProgress;
        
        return Result.Success();
    }
    
    /// <summary>
    /// Completes the maintenance task with actual hours and cost.
    /// </summary>
    public Result Complete(decimal actualHours, decimal actualCost, string completionNotes)
    {
        if (Status != MaintenanceTaskStatus.InProgress)
            return Result.Error($"Cannot complete task with status: {Status}");
        
        if (!StartedDate.HasValue)
            return Result.Error("Cannot complete task that hasn't been started");
        
        ActualHours = Ensure.NotNegative(actualHours, nameof(actualHours));
        ActualCost = Ensure.NotNegative(actualCost, nameof(actualCost));
        CompletionNotes = Ensure.NotEmpty(completionNotes, nameof(completionNotes));
        CompletedDate = DateTime.UtcNow;
        Status = MaintenanceTaskStatus.Completed;
        
        return Result.Success();
    }
    
    /// <summary>
    /// Cancels the maintenance task.
    /// </summary>
    public Result Cancel(string reason)
    {
        if (Status is MaintenanceTaskStatus.Completed or MaintenanceTaskStatus.Cancelled)
            return Result.Error($"Cannot cancel task with status: {Status}");
        
        CompletionNotes = $"Cancelled: {Ensure.NotEmpty(reason, nameof(reason))}";
        Status = MaintenanceTaskStatus.Cancelled;
        
        return Result.Success();
    }
    
    /// <summary>
    /// Adds a spare part to this maintenance task.
    /// </summary>
    public Result AddSparePart(Guid sparePartRef, int quantityUsed, decimal unitCost)
    {
        if (Status == MaintenanceTaskStatus.Completed)
            return Result.Error("Cannot add spare parts to completed task");
        
        var sparePart = MaintenanceTaskSparePart.Create(Id, sparePartRef, quantityUsed, unitCost);
        _spareParts.Add(sparePart);
        
        // Update actual cost
        ActualCost += quantityUsed * unitCost;
        
        return Result.Success();
    }
    
    /// <summary>
    /// Updates the scheduled date of the maintenance task.
    /// </summary>
    public Result Reschedule(DateTime newScheduledDate)
    {
        if (Status == MaintenanceTaskStatus.InProgress)
            return Result.Error("Cannot reschedule task that is in progress");
        
        if (Status is MaintenanceTaskStatus.Completed or MaintenanceTaskStatus.Cancelled)
            return Result.Error("Cannot reschedule completed or cancelled task");
        
        ScheduledDate = Ensure.NotDefault(newScheduledDate, nameof(newScheduledDate));
        
        return Result.Success();
    }
    
    /// <summary>
    /// Updates the priority of the maintenance task.
    /// </summary>
    public Result UpdatePriority(MaintenanceTaskPriority newPriority)
    {
        if (Status is MaintenanceTaskStatus.Completed or MaintenanceTaskStatus.Cancelled)
            return Result.Error("Cannot update priority of completed or cancelled task");
        
        // Business rule: Can only upgrade priority when in progress
        if (Status == MaintenanceTaskStatus.InProgress && newPriority < Priority)
            return Result.Error("Cannot downgrade priority of in-progress task");
        
        Priority = newPriority;
        
        return Result.Success();
    }
    
    /// <summary>
    /// Checks if the task is overdue.
    /// </summary>
    public bool IsOverdue()
    {
        return Status != MaintenanceTaskStatus.Completed 
               && Status != MaintenanceTaskStatus.Cancelled 
               && ScheduledDate < DateTime.UtcNow;
    }
    
    /// <summary>
    /// Calculates the variance between estimated and actual hours.
    /// </summary>
    public decimal CalculateHoursVariance()
    {
        if (Status != MaintenanceTaskStatus.Completed)
            return 0;
        
        return ActualHours - EstimatedHours;
    }
    
    /// <summary>
    /// Calculates the variance between estimated and actual cost.
    /// </summary>
    public decimal CalculateCostVariance()
    {
        if (Status != MaintenanceTaskStatus.Completed)
            return 0;
        
        return ActualCost - EstimatedCost;
    }
    
    #endregion
}

/// <summary>
/// Represents the spare parts used in a maintenance task.
/// </summary>
public sealed class MaintenanceTaskSparePart : BaseEntity
{
    public Guid MaintenanceTaskRef { get; init; }
    public Guid SparePartRef { get; init; }
    public int QuantityUsed { get; init; }
    public decimal UnitCost { get; init; }
    public decimal TotalCost { get; init; }
    
    [ForeignKey(nameof(MaintenanceTaskRef))]
    public MaintenanceTask MaintenanceTask { get; init; } = null!;
    
    [ForeignKey(nameof(SparePartRef))]
    public SparePart SparePart { get; init; } = null!;
    
    private MaintenanceTaskSparePart() { }
    
    private MaintenanceTaskSparePart(
        Guid maintenanceTaskRef,
        Guid sparePartRef,
        int quantityUsed,
        decimal unitCost)
    {
        MaintenanceTaskRef = Ensure.NotEmpty(maintenanceTaskRef, nameof(maintenanceTaskRef));
        SparePartRef = Ensure.NotEmpty(sparePartRef, nameof(sparePartRef));
        QuantityUsed = Ensure.Positive(quantityUsed, nameof(quantityUsed));
        UnitCost = Ensure.NotNegative(unitCost, nameof(unitCost));
        TotalCost = quantityUsed * unitCost;
    }
    
    public static MaintenanceTaskSparePart Create(
        Guid maintenanceTaskRef,
        Guid sparePartRef,
        int quantityUsed,
        decimal unitCost)
    {
        return new MaintenanceTaskSparePart(maintenanceTaskRef, sparePartRef, quantityUsed, unitCost);
    }
}


