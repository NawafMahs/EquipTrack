using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset is deactivated
/// </summary>
public sealed class AssetDeactivatedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public DateTime OccurredOn { get; }

    public AssetDeactivatedEvent(Guid assetId)
    {
        AssetId = assetId;
        OccurredOn = DateTime.UtcNow;
    }
}