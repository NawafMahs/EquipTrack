using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

public class WorkOrder : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
    public WorkOrderStatus Status { get; set; }
    public WorkOrderType Type { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public string? CompletionNotes { get; set; }

    // Foreign Keys
    public Guid AssetRef { get; set; }
    public Guid CreatedByUserRef { get; set; }
    public Guid? AssignedToUserRef { get; set; }

    // Navigation properties
    public virtual Asset Asset { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? AssignedToUser { get; set; }
    public virtual ICollection<WorkOrderSparePart> WorkOrderSpareParts { get; set; } = new List<WorkOrderSparePart>();
}