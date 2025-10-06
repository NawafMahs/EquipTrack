using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset is disposed
/// </summary>
public sealed class AssetDisposedEvent : BaseEvent 
{
    public Guid AssetId { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }

    public AssetDisposedEvent(Guid assetId, string reason)
    {
        AssetId = assetId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}