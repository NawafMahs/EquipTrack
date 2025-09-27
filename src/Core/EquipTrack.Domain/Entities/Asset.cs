using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

public class Asset : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public virtual ICollection<PreventiveMaintenance> PreventiveMaintenances { get; set; } = new List<PreventiveMaintenance>();
    public virtual ICollection<WorkOrderSparePart> WorkOrderSpareParts { get; set; } = new List<WorkOrderSparePart>();
}