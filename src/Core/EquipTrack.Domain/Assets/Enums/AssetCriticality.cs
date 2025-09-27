namespace EquipTrack.Domain.Assets.Enums;

/// <summary>
/// Represents the criticality level of an asset to business operations.
/// </summary>
public enum AssetCriticality
{
    /// <summary>
    /// Asset has low impact on operations if it fails.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Asset has moderate impact on operations if it fails.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Asset has high impact on operations if it fails.
    /// </summary>
    High = 3,

    /// <summary>
    /// Asset is critical to operations and failure would cause significant disruption.
    /// </summary>
    Critical = 4
}