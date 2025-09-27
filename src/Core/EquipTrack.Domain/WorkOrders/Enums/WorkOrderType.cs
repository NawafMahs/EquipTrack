namespace EquipTrack.Domain.WorkOrders.Enums;

/// <summary>
/// Represents the type of work order.
/// </summary>
public enum WorkOrderType
{
    /// <summary>
    /// Corrective maintenance to fix a problem.
    /// </summary>
    Corrective = 1,

    /// <summary>
    /// Preventive maintenance performed on a schedule.
    /// </summary>
    Preventive = 2,

    /// <summary>
    /// Emergency repair work.
    /// </summary>
    Emergency = 3,

    /// <summary>
    /// Routine inspection work.
    /// </summary>
    Inspection = 4,

    /// <summary>
    /// Installation of new equipment or components.
    /// </summary>
    Installation = 5,

    /// <summary>
    /// Modification or upgrade work.
    /// </summary>
    Modification = 6
}