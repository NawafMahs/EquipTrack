using EquipTrack.Domain.Enums;

using EquipTrack.Application.DTOs;
namespace EquipTrack.Application.DTOs;

public class WorkOrderQuery
{
    public Guid Id { get; set; }
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
    public DateTime CreatedAt { get; set; }

    // Related entities
    public AssetQuery Asset { get; set; } = null!;
    public UserQuery CreatedByUser { get; set; } = null!;
    public UserQuery? AssignedToUser { get; set; }
    public List<WorkOrderSparePartQuery> SpareParts { get; set; } = new();
}

public class WorkOrderSparePartQuery
{
    public Guid Id { get; set; }
    public Guid SparePartId { get; set; }
    public string SparePartName { get; set; } = string.Empty;
    public string SparePartPartNumber { get; set; } = string.Empty;
    public int QuantityUsed { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Notes { get; set; }
}