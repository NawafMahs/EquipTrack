using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset's criticality level changes
/// </summary>
public sealed class AssetCriticalityChangedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public AssetCriticality OldCriticality { get; }
    public AssetCriticality NewCriticality { get; }
    public DateTime OccurredOn { get; }

    public AssetCriticalityChangedEvent(
        Guid assetId, 
        AssetCriticality oldCriticality, 
        AssetCriticality newCriticality)
    {
        AssetId = assetId;
        OldCriticality = oldCriticality;
        NewCriticality = newCriticality;
        OccurredOn = DateTime.UtcNow;
    }
}