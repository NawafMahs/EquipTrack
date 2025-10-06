namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the type of machine log entry.
/// </summary>
[Obsolete("Use AssetLogType instead. This enum is deprecated and will be removed in a future version.")]
public enum MachineLogType
{
    StatusChange = 0,
    Error = 1,
    Warning = 2,
    Maintenance = 3,
    OperationStart = 4,
    OperationEnd = 5,
    ConfigurationChange = 6,
    SensorAlert = 7,
    SparePartReplacement = 8,
    Calibration = 9
}
