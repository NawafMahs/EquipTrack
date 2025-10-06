using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents spare part usage for an asset.
/// Unified entity replacing MachineSparePartUsage and RobotSparePartUsage.
/// Tracks which spare parts were used on which assets.
/// </summary>
public class AssetSparePartUsage : BaseEntity
{
    /// <summary>
    /// Foreign key to the asset
    /// </summary>
    public Guid AssetRef { get; private set; }

    /// <summary>
    /// Foreign key to the spare part
    /// </summary>
    public Guid SparePartRef { get; private set; }

    /// <summary>
    /// Quantity of spare parts used
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Date when the spare part was used
    /// </summary>
    public DateTime UsageDate { get; private set; }

    /// <summary>
    /// Optional foreign key to the work order
    /// </summary>
    public Guid? WorkOrderRef { get; private set; }

    /// <summary>
    /// Additional notes about the usage
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Cost per unit at the time of usage
    /// </summary>
    public decimal? UnitCost { get; private set; }

    /// <summary>
    /// Total cost (Quantity * UnitCost)
    /// </summary>
    public decimal? TotalCost { get; private set; }

    /// <summary>
    /// Navigation property to the asset
    /// </summary>
    public Asset Asset { get; private set; } = default!;

    /// <summary>
    /// Navigation property to the spare part
    /// </summary>
    public SparePart SparePart { get; private set; } = default!;

    /// <summary>
    /// Navigation property to the work order (optional)
    /// </summary>
    public WorkOrder? WorkOrder { get; private set; }

    // EF Core constructor
    protected AssetSparePartUsage() { }

    private AssetSparePartUsage(
        Guid assetId,
        Guid sparePartId,
        int quantity,
        Guid? workOrderId = null,
        string? notes = null,
        decimal? unitCost = null)
    {
        AssetRef = assetId;
        SparePartRef = sparePartId;
        Quantity = Ensure.Positive(quantity, nameof(quantity));
        UsageDate = DateTime.UtcNow;
        WorkOrderRef = workOrderId;
        Notes = notes;
        UnitCost = unitCost;
        
        if (unitCost.HasValue)
        {
            TotalCost = quantity * unitCost.Value;
        }
    }

    /// <summary>
    /// Factory method to create a new spare part usage record
    /// </summary>
    public static AssetSparePartUsage Create(
        Guid assetId,
        Guid sparePartId,
        int quantity,
        Guid? workOrderId = null,
        string? notes = null,
        decimal? unitCost = null)
    {
        return new AssetSparePartUsage(assetId, sparePartId, quantity, workOrderId, notes, unitCost);
    }

    /// <summary>
    /// Updates the quantity used
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        Quantity = Ensure.Positive(newQuantity, nameof(newQuantity));
        
        if (UnitCost.HasValue)
        {
            TotalCost = newQuantity * UnitCost.Value;
        }
    }

    /// <summary>
    /// Sets the unit cost and calculates total cost
    /// </summary>
    public void SetCost(decimal unitCost)
    {
        UnitCost = Ensure.Positive(unitCost, nameof(unitCost));
        TotalCost = Quantity * unitCost;
    }

    /// <summary>
    /// Adds or updates notes
    /// </summary>
    public void AddNotes(string notes)
    {
        Notes = string.IsNullOrWhiteSpace(Notes)
            ? notes
            : $"{Notes}\n{notes}";
    }

    public override string ToString()
    {
        return $"Spare Part Usage: {Quantity} units on {UsageDate:yyyy-MM-dd}";
    }
}