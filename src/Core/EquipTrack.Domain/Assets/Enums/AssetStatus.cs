namespace EquipTrack.Domain.Assets.Enums;

/// <summary>
/// Represents the operational status of an asset.
/// </summary>
public enum AssetStatus
{
    /// <summary>
    /// Asset is operational and available for use.
    /// </summary>
    Operational = 1,

    /// <summary>
    /// Asset is temporarily out of service for maintenance.
    /// </summary>
    UnderMaintenance = 2,

    /// <summary>
    /// Asset is broken and requires repair.
    /// </summary>
    OutOfOrder = 3,

    /// <summary>
    /// Asset has been retired and is no longer in use.
    /// </summary>
    Retired = 4,

    /// <summary>
    /// Asset is in storage and not currently deployed.
    /// </summary>
    InStorage = 5
}