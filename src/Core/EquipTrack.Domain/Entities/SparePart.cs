using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Entities;

public class SparePart : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public string Unit { get; set; } = string.Empty; // e.g., "pieces", "liters", "kg"
    public string? Location { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ICollection<WorkOrderSparePart> WorkOrderSpareParts { get; set; } = new List<WorkOrderSparePart>();

    public bool IsLowStock => QuantityInStock <= MinimumStock;
}