using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.WorkOrders.Events;

/// <summary>
/// Domain event raised when a new work order is created.
/// </summary>
public class WorkOrderCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkOrderCreatedEvent"/> class.
    /// </summary>
    /// <param name="workOrderId">The ID of the created work order.</param>
    /// <param name="workOrderNumber">The work order number.</param>
    /// <param name="title">The title of the work order.</param>
    /// <param name="assetId">The ID of the associated asset.</param>
    public WorkOrderCreatedEvent(Guid workOrderId, string workOrderNumber, string title, Guid assetId)
    {
        WorkOrderId = workOrderId;
        WorkOrderNumber = workOrderNumber;
        Title = title;
        AssetId = assetId;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the created work order.
    /// </summary>
    public Guid WorkOrderId { get; }

    /// <summary>
    /// Gets the work order number.
    /// </summary>
    public string WorkOrderNumber { get; }

    /// <summary>
    /// Gets the title of the work order.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the ID of the associated asset.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}