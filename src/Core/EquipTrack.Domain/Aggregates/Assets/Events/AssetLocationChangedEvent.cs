using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset's location changes
/// </summary>
public sealed class AssetLocationChangedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public string OldLocation { get; }
    public string NewLocation { get; }
    public DateTime OccurredOn { get; }

    public AssetLocationChangedEvent(Guid assetId, string oldLocation, string newLocation)
    {
        AssetId = assetId;
        OldLocation = oldLocation;
        NewLocation = newLocation;
        OccurredOn = DateTime.UtcNow;
    }
}