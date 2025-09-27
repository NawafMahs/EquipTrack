using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Entities;

public class WorkOrderSparePart : BaseEntity
{
    public Guid WorkOrderRef { get; set; }
    public Guid SparePartRef { get; set; }
    public Guid? AssetRef { get; set; }
    public int QuantityUsed { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost => QuantityUsed * UnitCost;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual WorkOrder WorkOrder { get; set; } = null!;
    public virtual SparePart SparePart { get; set; } = null!;
    public virtual Asset? Asset { get; set; }
}