using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset is activated
/// </summary>
public sealed class AssetActivatedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public DateTime OccurredOn { get; }

    public AssetActivatedEvent(Guid assetId)
    {
        AssetId = assetId;
        OccurredOn = DateTime.UtcNow;
    }
}