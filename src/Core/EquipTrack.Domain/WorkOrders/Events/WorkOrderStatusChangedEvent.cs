using EquipTrack.Domain.Common;
using EquipTrack.Domain.WorkOrders.Enums;

namespace EquipTrack.Domain.WorkOrders.Events;

/// <summary>
/// Domain event raised when a work order's status changes.
/// </summary>
public class WorkOrderStatusChangedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkOrderStatusChangedEvent"/> class.
    /// </summary>
    /// <param name="workOrderId">The ID of the work order.</param>
    /// <param name="previousStatus">The previous status.</param>
    /// <param name="newStatus">The new status.</param>
    public WorkOrderStatusChangedEvent(Guid workOrderId, WorkOrderStatus previousStatus, WorkOrderStatus newStatus)
    {
        WorkOrderId = workOrderId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the work order.
    /// </summary>
    public Guid WorkOrderId { get; }

    /// <summary>
    /// Gets the previous status.
    /// </summary>
    public WorkOrderStatus PreviousStatus { get; }

    /// <summary>
    /// Gets the new status.
    /// </summary>
    public WorkOrderStatus NewStatus { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}