using EquipTrack.Domain.Assets.Enums;
using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Assets.Events;

/// <summary>
/// Domain event raised when an asset's status changes.
/// </summary>
public class AssetStatusChangedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetStatusChangedEvent"/> class.
    /// </summary>
    /// <param name="assetId">The ID of the asset.</param>
    /// <param name="previousStatus">The previous status.</param>
    /// <param name="newStatus">The new status.</param>
    public AssetStatusChangedEvent(Guid assetId, AssetStatus previousStatus, AssetStatus newStatus)
    {
        AssetId = assetId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the ID of the asset.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Gets the previous status.
    /// </summary>
    public AssetStatus PreviousStatus { get; }

    /// <summary>
    /// Gets the new status.
    /// </summary>
    public AssetStatus NewStatus { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; }
}