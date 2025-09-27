using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.WorkOrders.Events;

/// <summary>
/// Domain event raised when a work order is assigned to a technician.
/// </summary>
public class WorkOrderAssignedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkOrderAssignedEvent"/> class.
    /// </summary>
    /// <param name="workOrderId">The ID of the work order.</param>
    /// <param name="technicianId">The ID of the assigned technician.</param>
    /// <param name="previousTechnicianId">The ID of the previously assigned technician, if any.</param>
    public WorkOrderAssignedEvent(Guid workOrderId, Guid technicianId, Guid? previousTechnicianId)
    {
        WorkOrderId = workOrderId;
        TechnicianId = technicianId;
        PreviousTechnicianId = previousTechnicianId;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the work order.
    /// </summary>
    public Guid WorkOrderId { get; }

    /// <summary>
    /// Gets the ID of the assigned technician.
    /// </summary>
    public Guid TechnicianId { get; }

    /// <summary>
    /// Gets the ID of the previously assigned technician, if any.
    /// </summary>
    public Guid? PreviousTechnicianId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}