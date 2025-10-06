using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when a note is added to an asset
/// </summary>
public sealed class AssetNoteAddedEvent : BaseEvent
{
    public Guid AssetId { get; }
    public string Note { get; }
    public DateTime OccurredOn { get; }

    public AssetNoteAddedEvent(Guid assetId, string note)
    {
        AssetId = assetId;
        Note = note;
        OccurredOn = DateTime.UtcNow;
    }
}