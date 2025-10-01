using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents an operational log entry for a machine.
/// Tracks all events, errors, and status changes.
/// </summary>
public class MachineLog : BaseEntity
{
    public Guid MachineRef { get; init; }
    public MachineLogType LogType { get; init; }
    public string Message { get; init; } = default!;
    public MachineLogSeverity Severity { get; init; }
    public DateTime Timestamp { get; init; }
    public string? AdditionalData { get; init; }

    // EF Core constructor
    protected MachineLog() { }

    private MachineLog(
        Guid machineRef,
        MachineLogType logType,
        string message,
        MachineLogSeverity severity,
        DateTime? timestamp = null,
        string? additionalData = null)
    {
        MachineRef = machineRef;
        LogType = logType;
        Message = Ensure.NotEmpty(message, nameof(message));
        Severity = severity;
        Timestamp = timestamp ?? DateTime.UtcNow;
        AdditionalData = additionalData;
    }

    public static MachineLog Create(
        Guid machineRef,
        MachineLogType logType,
        string message,
        MachineLogSeverity severity,
        DateTime? timestamp = null,
        string? additionalData = null)
    {
        return new MachineLog(machineRef, logType, message, severity, timestamp, additionalData);
    }
}
