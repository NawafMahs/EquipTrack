using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.WorkOrders.Events;

/// <summary>
/// Domain event raised when a work order is completed.
/// </summary>
public class WorkOrderCompletedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkOrderCompletedEvent"/> class.
    /// </summary>
    /// <param name="workOrderId">The ID of the work order.</param>
    /// <param name="completedAt">The date and time when the work order was completed.</param>
    public WorkOrderCompletedEvent(Guid workOrderId, DateTime completedAt)
    {
        WorkOrderId = workOrderId;
        CompletedAt = completedAt;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the work order.
    /// </summary>
    public Guid WorkOrderId { get; }

    /// <summary>
    /// Gets the date and time when the work order was completed.
    /// </summary>
    public DateTime CompletedAt { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}