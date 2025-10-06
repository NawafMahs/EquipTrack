using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when maintenance is recorded for an asset
/// </summary>
public sealed class AssetMaintenanceRecordedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public DateTime MaintenanceDate { get; }
    public DateTime? NextScheduledDate { get; }
    public string? Notes { get; }
    public DateTime OccurredOn { get; }

    public AssetMaintenanceRecordedEvent(
        Guid assetId, 
        DateTime maintenanceDate, 
        DateTime? nextScheduledDate, 
        string? notes)
    {
        AssetId = assetId;
        MaintenanceDate = maintenanceDate;
        NextScheduledDate = nextScheduledDate;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
    }
}