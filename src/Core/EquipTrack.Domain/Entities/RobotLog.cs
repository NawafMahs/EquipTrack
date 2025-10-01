using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents an operational log entry for a robot.
/// Tracks all events, errors, and status changes.
/// </summary>
public class RobotLog : BaseEntity
{
    public Guid RobotRef { get; init; }
    public RobotLogType LogType { get; init; }
    public string Message { get; init; } = default!;
    public RobotLogSeverity Severity { get; init; }
    public DateTime Timestamp { get; init; }
    public string? AdditionalData { get; init; }

    // EF Core constructor
    protected RobotLog() { }

    private RobotLog(
        Guid robotRef,
        RobotLogType logType,
        string message,
        RobotLogSeverity severity,
        DateTime? timestamp = null,
        string? additionalData = null)
    {
        RobotRef = robotRef;
        LogType = logType;
        Message = Ensure.NotEmpty(message, nameof(message));
        Severity = severity;
        Timestamp = timestamp ?? DateTime.UtcNow;
        AdditionalData = additionalData;
    }

    public static RobotLog Create(
        Guid robotRef,
        RobotLogType logType,
        string message,
        RobotLogSeverity severity,
        DateTime? timestamp = null,
        string? additionalData = null)
    {
        return new RobotLog(robotRef, logType, message, severity, timestamp, additionalData);
    }
}
