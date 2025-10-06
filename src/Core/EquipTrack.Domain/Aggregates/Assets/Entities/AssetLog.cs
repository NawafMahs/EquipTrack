using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a log entry for an asset.
/// Unified log entity replacing MachineLog and RobotLog.
/// </summary>
public class AssetLog : BaseEntity
{
    /// <summary>
    /// Foreign key to the asset
    /// </summary>
    public Guid AssetId { get; private set; }

    /// <summary>
    /// Type of log entry
    /// </summary>
    public AssetLogType LogType { get; private set; }

    /// <summary>
    /// Severity level of the log
    /// </summary>
    public LogSeverity Severity { get; private set; }

    /// <summary>
    /// Log message content
    /// </summary>
    public string Message { get; private set; } = default!;

    /// <summary>
    /// Timestamp when the log was created
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Source of the log (e.g., RabbitMQ, MQTT, Manual)
    /// </summary>
    public string? Source { get; private set; }

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public Dictionary<string, string>? Metadata { get; private set; }

    /// <summary>
    /// Navigation property to the asset
    /// </summary>
    public Asset Asset { get; private set; } = default!;

    // EF Core constructor
    protected AssetLog() { }

    private AssetLog(
        Guid assetId,
        AssetLogType logType,
        string message,
        LogSeverity severity,
        string? source = null)
    {
        AssetId = assetId;
        LogType = logType;
        Message = Ensure.NotEmpty(message, nameof(message));
        Severity = severity;
        Timestamp = DateTime.UtcNow;
        Source = source;
    }

    /// <summary>
    /// Factory method to create a new asset log
    /// </summary>
    public static AssetLog Create(
        Guid assetId,
        AssetLogType logType,
        string message,
        LogSeverity severity,
        string? source = null)
    {
        return new AssetLog(assetId, logType, message, severity, source);
    }

    /// <summary>
    /// Adds metadata to the log entry
    /// </summary>
    public void AddMetadata(string key, string value)
    {
        Metadata ??= new Dictionary<string, string>();
        Metadata[key] = value;
    }

    public override string ToString()
    {
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Severity} - {LogType}: {Message}";
    }
}