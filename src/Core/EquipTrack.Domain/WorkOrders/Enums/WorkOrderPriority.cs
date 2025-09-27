namespace EquipTrack.Domain.WorkOrders.Enums;

/// <summary>
/// Represents the priority level of a work order.
/// </summary>
public enum WorkOrderPriority
{
    /// <summary>
    /// Low priority work that can be scheduled flexibly.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Normal priority work.
    /// </summary>
    Normal = 2,

    /// <summary>
    /// High priority work that should be addressed soon.
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical work that must be addressed immediately.
    /// </summary>
    Critical = 4,

    /// <summary>
    /// Emergency work that requires immediate attention.
    /// </summary>
    Emergency = 5
}