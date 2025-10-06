namespace EquipTrack.Domain.Enums;

/// <summary>
/// Represents the severity level of a machine log entry.
/// </summary>
[Obsolete("Use LogSeverity instead. This enum is deprecated and will be removed in a future version.")]
public enum MachineLogSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3
}
