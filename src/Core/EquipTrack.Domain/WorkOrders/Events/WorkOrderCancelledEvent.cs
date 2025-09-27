using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.WorkOrders.Events;

/// <summary>
/// Domain event raised when a work order is cancelled.
/// </summary>
public class WorkOrderCancelledEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkOrderCancelledEvent"/> class.
    /// </summary>
    /// <param name="workOrderId">The ID of the work order.</param>
    /// <param name="reason">The reason for cancellation.</param>
    public WorkOrderCancelledEvent(Guid workOrderId, string? reason)
    {
        WorkOrderId = workOrderId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the work order.
    /// </summary>
    public Guid WorkOrderId { get; }

    /// <summary>
    /// Gets the reason for cancellation.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}