namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the severity level of a robot log entry.
/// </summary>
[Obsolete("Use LogSeverity instead. This enum is deprecated and will be removed in a future version.")]
public enum RobotLogSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3
}
