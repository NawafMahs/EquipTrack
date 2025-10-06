using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when an asset's status changes
/// </summary>
public sealed class AssetStatusChangedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public AssetStatus OldStatus { get; }
    public AssetStatus NewStatus { get; }
    public DateTime OccurredOn { get; }

    public AssetStatusChangedEvent(Guid assetId, AssetStatus oldStatus, AssetStatus newStatus)
    {
        AssetId = assetId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }
}