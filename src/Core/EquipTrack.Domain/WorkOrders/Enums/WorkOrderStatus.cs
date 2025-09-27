namespace EquipTrack.Domain.WorkOrders.Enums;

/// <summary>
/// Represents the status of a work order.
/// </summary>
public enum WorkOrderStatus
{
    /// <summary>
    /// Work order has been created but not yet assigned.
    /// </summary>
    Open = 1,

    /// <summary>
    /// Work order has been assigned to a technician.
    /// </summary>
    Assigned = 2,

    /// <summary>
    /// Work is currently in progress.
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Work has been completed and is awaiting review.
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Work order has been closed after review.
    /// </summary>
    Closed = 5,

    /// <summary>
    /// Work order has been cancelled.
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Work order is on hold pending additional information or resources.
    /// </summary>
    OnHold = 7
}