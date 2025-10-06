using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when a spare part is used on an asset
/// </summary>
public sealed class AssetSparePartUsedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public Guid SparePartId { get; }
    public int Quantity { get; }
    public Guid? WorkOrderId { get; }
    public string? Notes { get; }
    public DateTime OccurredOn { get; }

    public AssetSparePartUsedEvent(
        Guid assetId, 
        Guid sparePartId, 
        int quantity, 
        Guid? workOrderId, 
        string? notes)
    {
        AssetId = assetId;
        SparePartId = sparePartId;
        Quantity = quantity;
        WorkOrderId = workOrderId;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
    }
}