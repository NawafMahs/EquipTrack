using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

public class WorkOrder : BaseEntity
{
    // Properties
    public string Title { get; private set; } = default!;
    public WorkOrderStatus Status { get; private set; } = default!;
    public DateTime RequestedDate { get; private set; }
    public DateTime? ScheduledDate { get; private set; }
    public DateTime? StartedDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public string? CompletionNotes { get; private set; }

    // EF Core navigation
    public Guid AssetRef { get; private set; }
    public virtual Asset Asset { get; private set; } = default!;

    // Private constructor for EF Core
    protected WorkOrder() { }

    // Internal constructor for controlled creation via Aggregate Root
    internal WorkOrder(string title, DateTime requestedDate, Guid assetRef)
    {
        Title = Ensure.NotEmpty(title, nameof(title));
        RequestedDate = requestedDate;
        Status = WorkOrderStatus.Open;
        AssetRef = assetRef;
    }

    // Factory method
    public static WorkOrder Create(string title, DateTime requestedDate, Guid assetRef)
        => new(title, requestedDate, assetRef);

    #region Behavior Methods

    public void AssignToUser(Guid userId)
    {
        if (Status != WorkOrderStatus.Open)
            throw new InvalidOperationException("Work order must be Open to assign.");
        Status = WorkOrderStatus.Assigned;
        // يمكن إضافة AssignToUserRef هنا إذا أردنا تتبع الشخص المسؤول
    }

    public void StartWork()
    {
        if (Status != WorkOrderStatus.Assigned && Status != WorkOrderStatus.OnHold)
            throw new InvalidOperationException("Cannot start work unless assigned or on hold.");
        Status = WorkOrderStatus.InProgress;
        StartedDate = DateTime.UtcNow;
    }

    public void PutOnHold()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Only InProgress work can be put on hold.");
        Status = WorkOrderStatus.OnHold;
    }

    public void CompleteWork(string? completionNotes = null)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Work must be in progress to complete.");
        Status = WorkOrderStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        CompletionNotes = completionNotes;
    }

    public void CancelWork(string? reason = null)
    {
        if (Status is WorkOrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed work order.");
        Status = WorkOrderStatus.Cancelled;
        CompletionNotes = reason;
    }

    #endregion

    #region Optional Enhancements

    // Example of a domain event trigger placeholder
    // internal void RaiseWorkOrderStartedEvent() { ... }

    #endregion
}
