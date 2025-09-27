using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Assets.Events;

/// <summary>
/// Domain event raised when a new asset is created.
/// </summary>
public class AssetCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetCreatedEvent"/> class.
    /// </summary>
    /// <param name="assetId">The ID of the created asset.</param>
    /// <param name="assetName">The name of the created asset.</param>
    /// <param name="assetTag">The tag of the created asset.</param>
    public AssetCreatedEvent(Guid assetId, string assetName, string assetTag)
    {
        AssetId = assetId;
        AssetName = assetName;
        AssetTag = assetTag;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the created asset.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Gets the name of the created asset.
    /// </summary>
    public string AssetName { get; }

    /// <summary>
    /// Gets the tag of the created asset.
    /// </summary>
    public string AssetTag { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}