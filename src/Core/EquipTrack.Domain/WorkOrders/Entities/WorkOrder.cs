using EquipTrack.Domain.Assets.Entities;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.WorkOrders.Enums;
using EquipTrack.Domain.WorkOrders.Events;

namespace EquipTrack.Domain.WorkOrders.Entities;

/// <summary>
/// Represents a work order for maintenance activities.
/// </summary>
public class WorkOrder : AggregateRoot
{
    /// <summary>
    /// Gets the work order number.
    /// </summary>
    public string WorkOrderNumber { get; private set; } = default!;

    /// <summary>
    /// Gets the title of the work order.
    /// </summary>
    public string Title { get; private set; } = default!;

    /// <summary>
    /// Gets the description of the work to be performed.
    /// </summary>
    public string Description { get; private set; } = default!;

    /// <summary>
    /// Gets the type of work order.
    /// </summary>
    public WorkOrderType Type { get; private set; }

    /// <summary>
    /// Gets the priority level of the work order.
    /// </summary>
    public WorkOrderPriority Priority { get; private set; }

    /// <summary>
    /// Gets the current status of the work order.
    /// </summary>
    public WorkOrderStatus Status { get; private set; }

    /// <summary>
    /// Gets the ID of the asset this work order is for.
    /// </summary>
    public Guid AssetId { get; private set; }

    /// <summary>
    /// Gets the asset this work order is for.
    /// </summary>
    public Asset Asset { get; private set; } = default!;

    /// <summary>
    /// Gets the ID of the technician assigned to this work order.
    /// </summary>
    public Guid? AssignedTechnicianId { get; private set; }

    /// <summary>
    /// Gets the ID of the user who requested this work order.
    /// </summary>
    public Guid RequestedById { get; private set; }

    /// <summary>
    /// Gets the date when the work order was requested.
    /// </summary>
    public DateTime RequestedDate { get; private set; }

    /// <summary>
    /// Gets the scheduled start date for the work.
    /// </summary>
    public DateTime? ScheduledStartDate { get; private set; }

    /// <summary>
    /// Gets the scheduled completion date for the work.
    /// </summary>
    public DateTime? ScheduledCompletionDate { get; private set; }

    /// <summary>
    /// Gets the actual start date of the work.
    /// </summary>
    public DateTime? ActualStartDate { get; private set; }

    /// <summary>
    /// Gets the actual completion date of the work.
    /// </summary>
    public DateTime? ActualCompletionDate { get; private set; }

    /// <summary>
    /// Gets the estimated hours required for the work.
    /// </summary>
    public decimal? EstimatedHours { get; private set; }

    /// <summary>
    /// Gets the actual hours spent on the work.
    /// </summary>
    public decimal? ActualHours { get; private set; }

    /// <summary>
    /// Gets the estimated cost for the work.
    /// </summary>
    public decimal? EstimatedCost { get; private set; }

    /// <summary>
    /// Gets the actual cost of the work.
    /// </summary>
    public decimal? ActualCost { get; private set; }

    /// <summary>
    /// Gets the completion notes.
    /// </summary>
    public string? CompletionNotes { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkOrder"/> class.
    /// </summary>
    /// <param name="workOrderNumber">The work order number.</param>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="type">The work order type.</param>
    /// <param name="priority">The priority level.</param>
    /// <param name="assetId">The asset ID.</param>
    /// <param name="requestedById">The ID of the user who requested the work order.</param>
    private WorkOrder(
        string workOrderNumber,
        string title,
        string description,
        WorkOrderType type,
        WorkOrderPriority priority,
        Guid assetId,
        Guid requestedById)
    {
        SetWorkOrderNumber(workOrderNumber);
        SetTitle(title);
        SetDescription(description);
        SetType(type);
        SetPriority(priority);
        AssetId = assetId;
        RequestedById = requestedById;

        Status = WorkOrderStatus.Open;
        RequestedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderCreatedEvent(Id, workOrderNumber, title, assetId));
    }

    /// <summary>
    /// Protected parameterless constructor for ORM materialization.
    /// </summary>
    protected WorkOrder() { }

    /// <summary>
    /// Creates a new work order with the specified details.
    /// </summary>
    /// <param name="workOrderNumber">The work order number.</param>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="type">The work order type.</param>
    /// <param name="priority">The priority level.</param>
    /// <param name="assetId">The asset ID.</param>
    /// <param name="requestedById">The ID of the user who requested the work order.</param>
    /// <returns>A new WorkOrder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when inputs are invalid.</exception>
    public static WorkOrder Create(
        string workOrderNumber,
        string title,
        string description,
        WorkOrderType type,
        WorkOrderPriority priority,
        Guid assetId,
        Guid requestedById)
    {
        ValidateWorkOrderData(workOrderNumber, title, description);

        if (assetId == Guid.Empty)
            throw new ArgumentException("Asset ID cannot be empty.", nameof(assetId));

        if (requestedById == Guid.Empty)
            throw new ArgumentException("Requested by ID cannot be empty.", nameof(requestedById));

        return new WorkOrder(
            workOrderNumber.Trim(),
            title.Trim(),
            description.Trim(),
            type,
            priority,
            assetId,
            requestedById);
    }

    /// <summary>
    /// Validates work order data for creation or updates.
    /// </summary>
    private static void ValidateWorkOrderData(string workOrderNumber, string title, string description)
    {
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            throw new ArgumentException("Work order number must not be empty.", nameof(workOrderNumber));

        if (workOrderNumber.Length > 50)
            throw new ArgumentException("Work order number cannot exceed 50 characters.", nameof(workOrderNumber));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must not be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must not be empty.", nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters.", nameof(description));
    }

    /// <summary>
    /// Assigns the work order to a technician.
    /// </summary>
    /// <param name="technicianId">The technician ID.</param>
    /// <exception cref="ArgumentException">Thrown when technician ID is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when work order cannot be assigned.</exception>
    public void AssignTechnician(Guid technicianId)
    {
        if (technicianId == Guid.Empty)
            throw new ArgumentException("Technician ID cannot be empty.", nameof(technicianId));

        if (Status == WorkOrderStatus.Closed || Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot assign technician to a closed or cancelled work order.");

        var previousTechnicianId = AssignedTechnicianId;
        AssignedTechnicianId = technicianId;
        Status = WorkOrderStatus.Assigned;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderAssignedEvent(Id, technicianId, previousTechnicianId));
    }

    /// <summary>
    /// Starts the work order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when work order cannot be started.</exception>
    public void StartWork()
    {
        if (Status != WorkOrderStatus.Assigned)
            throw new InvalidOperationException("Work order must be assigned before it can be started.");

        Status = WorkOrderStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderStatusChangedEvent(Id, WorkOrderStatus.Assigned, WorkOrderStatus.InProgress));
    }

    /// <summary>
    /// Completes the work order.
    /// </summary>
    /// <param name="completionNotes">Notes about the completion.</param>
    /// <param name="actualHours">The actual hours spent.</param>
    /// <param name="actualCost">The actual cost incurred.</param>
    /// <exception cref="InvalidOperationException">Thrown when work order cannot be completed.</exception>
    public void CompleteWork(string? completionNotes = null, decimal? actualHours = null, decimal? actualCost = null)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Work order must be in progress before it can be completed.");

        if (actualHours.HasValue && actualHours.Value < 0)
            throw new ArgumentException("Actual hours cannot be negative.", nameof(actualHours));

        if (actualCost.HasValue && actualCost.Value < 0)
            throw new ArgumentException("Actual cost cannot be negative.", nameof(actualCost));

        Status = WorkOrderStatus.Completed;
        ActualCompletionDate = DateTime.UtcNow;
        CompletionNotes = completionNotes?.Trim();
        ActualHours = actualHours;
        ActualCost = actualCost;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderCompletedEvent(Id, ActualCompletionDate.Value));
    }

    /// <summary>
    /// Closes the work order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when work order cannot be closed.</exception>
    public void CloseWorkOrder()
    {
        if (Status != WorkOrderStatus.Completed)
            throw new InvalidOperationException("Work order must be completed before it can be closed.");

        Status = WorkOrderStatus.Closed;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderStatusChangedEvent(Id, WorkOrderStatus.Completed, WorkOrderStatus.Closed));
    }

    /// <summary>
    /// Cancels the work order.
    /// </summary>
    /// <param name="reason">The reason for cancellation.</param>
    /// <exception cref="InvalidOperationException">Thrown when work order cannot be cancelled.</exception>
    public void CancelWorkOrder(string? reason = null)
    {
        if (Status == WorkOrderStatus.Closed || Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a closed or already cancelled work order.");

        if (Status == WorkOrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed work order.");

        Status = WorkOrderStatus.Cancelled;
        CompletionNotes = reason?.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderCancelledEvent(Id, reason));
    }

    /// <summary>
    /// Puts the work order on hold.
    /// </summary>
    /// <param name="reason">The reason for putting on hold.</param>
    /// <exception cref="InvalidOperationException">Thrown when work order cannot be put on hold.</exception>
    public void PutOnHold(string? reason = null)
    {
        if (Status == WorkOrderStatus.Closed || Status == WorkOrderStatus.Cancelled || Status == WorkOrderStatus.Completed)
            throw new InvalidOperationException("Cannot put a closed, cancelled, or completed work order on hold.");

        var previousStatus = Status;
        Status = WorkOrderStatus.OnHold;
        CompletionNotes = reason?.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkOrderStatusChangedEvent(Id, previousStatus, WorkOrderStatus.OnHold));
    }

    /// <summary>
    /// Updates the work order number.
    /// </summary>
    /// <param name="workOrderNumber">The new work order number.</param>
    /// <exception cref="ArgumentException">Thrown when work order number is invalid.</exception>
    public void SetWorkOrderNumber(string workOrderNumber)
    {
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            throw new ArgumentException("Work order number must not be empty.", nameof(workOrderNumber));

        if (workOrderNumber.Length > 50)
            throw new ArgumentException("Work order number cannot exceed 50 characters.", nameof(workOrderNumber));

        WorkOrderNumber = workOrderNumber.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the title.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <exception cref="ArgumentException">Thrown when title is invalid.</exception>
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must not be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

        Title = title.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the description.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <exception cref="ArgumentException">Thrown when description is invalid.</exception>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must not be empty.", nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters.", nameof(description));

        Description = description.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the work order type.
    /// </summary>
    /// <param name="type">The new type.</param>
    public void SetType(WorkOrderType type)
    {
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the priority.
    /// </summary>
    /// <param name="priority">The new priority.</param>
    public void SetPriority(WorkOrderPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the scheduled dates.
    /// </summary>
    /// <param name="scheduledStartDate">The scheduled start date.</param>
    /// <param name="scheduledCompletionDate">The scheduled completion date.</param>
    /// <exception cref="ArgumentException">Thrown when dates are invalid.</exception>
    public void SetScheduledDates(DateTime? scheduledStartDate, DateTime? scheduledCompletionDate)
    {
        if (scheduledStartDate.HasValue && scheduledCompletionDate.HasValue &&
            scheduledStartDate.Value > scheduledCompletionDate.Value)
        {
            throw new ArgumentException("Scheduled start date cannot be after scheduled completion date.");
        }

        ScheduledStartDate = scheduledStartDate;
        ScheduledCompletionDate = scheduledCompletionDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the estimated hours and cost.
    /// </summary>
    /// <param name="estimatedHours">The estimated hours.</param>
    /// <param name="estimatedCost">The estimated cost.</param>
    /// <exception cref="ArgumentException">Thrown when values are invalid.</exception>
    public void SetEstimates(decimal? estimatedHours, decimal? estimatedCost)
    {
        if (estimatedHours.HasValue && estimatedHours.Value < 0)
            throw new ArgumentException("Estimated hours cannot be negative.", nameof(estimatedHours));

        if (estimatedCost.HasValue && estimatedCost.Value < 0)
            throw new ArgumentException("Estimated cost cannot be negative.", nameof(estimatedCost));

        EstimatedHours = estimatedHours;
        EstimatedCost = estimatedCost;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the work order is overdue.
    /// </summary>
    /// <returns>True if the work order is overdue, false otherwise.</returns>
    public bool IsOverdue()
    {
        return ScheduledCompletionDate.HasValue &&
               ScheduledCompletionDate.Value < DateTime.UtcNow &&
               Status != WorkOrderStatus.Completed &&
               Status != WorkOrderStatus.Closed &&
               Status != WorkOrderStatus.Cancelled;
    }

    /// <summary>
    /// Checks if the work order is in progress.
    /// </summary>
    /// <returns>True if the work order is in progress, false otherwise.</returns>
    public bool IsInProgress()
    {
        return Status == WorkOrderStatus.InProgress;
    }

    /// <summary>
    /// Checks if the work order is completed.
    /// </summary>
    /// <returns>True if the work order is completed, false otherwise.</returns>
    public bool IsCompleted()
    {
        return Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Closed;
    }
}