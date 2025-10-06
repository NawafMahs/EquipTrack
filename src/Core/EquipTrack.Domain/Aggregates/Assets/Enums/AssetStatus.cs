namespace EquipTrack.Domain.Enums;

/// <summary>
/// Unified status enum for all asset types.
/// Replaces MachineStatus and RobotStatus.
/// </summary>
public enum AssetStatus
{
    /// <summary>
    /// Asset is active and available
    /// </summary>
    Active = 0,

    /// <summary>
    /// Asset is idle (not currently in use)
    /// </summary>
    Idle = 1,

    /// <summary>
    /// Asset is currently running/operating
    /// </summary>
    Running = 2,

    /// <summary>
    /// Asset is under maintenance
    /// </summary>
    UnderMaintenance = 3,

    /// <summary>
    /// Asset has an error condition
    /// </summary>
    Error = 4,

    /// <summary>
    /// Asset is out of service
    /// </summary>
    OutOfService = 5,

    /// <summary>
    /// Asset is charging (for robots/electric vehicles)
    /// </summary>
    Charging = 6,

    /// <summary>
    /// Asset has been disposed
    /// </summary>
    Disposed = 7,

    /// <summary>
    /// Asset is inactive
    /// </summary>
    Inactive = 8
}