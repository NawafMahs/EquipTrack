namespace EquipTrack.Domain.Enums;

/// <summary>
/// Severity level for log entries.
/// Used across all asset log types.
/// </summary>
public enum LogSeverity
{
    /// <summary>
    /// Informational message
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning message
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error message
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical error message
    /// </summary>
    Critical = 3,

    /// <summary>
    /// Debug message
    /// </summary>
    Debug = 4
}