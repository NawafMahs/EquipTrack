namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the type of robot log entry.
/// </summary>
[Obsolete("Use AssetLogType instead. This enum is deprecated and will be removed in a future version.")]
public enum RobotLogType
{
    StatusChange = 0,
    Error = 1,
    Warning = 2,
    Maintenance = 3,
    TaskStart = 4,
    TaskComplete = 5,
    BatteryAlert = 6,
    ConfigurationChange = 7,
    SensorAlert = 8,
    SparePartReplacement = 9,
    Calibration = 10,
    CollisionDetected = 11
}
