using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when a sensor is attached to an asset
/// </summary>
public sealed class AssetSensorAttachedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public Guid SensorId { get; }
    public string MountLocation { get; }
    public DateTime OccurredOn { get; }

    public AssetSensorAttachedEvent(Guid assetId, Guid sensorId, string mountLocation)
    {
        AssetId = assetId;
        SensorId = sensorId;
        MountLocation = mountLocation;
        OccurredOn = DateTime.UtcNow;
    }
}