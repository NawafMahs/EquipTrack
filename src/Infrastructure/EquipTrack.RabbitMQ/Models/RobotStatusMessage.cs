using EquipTrack.Domain.Enums;

namespace EquipTrack.RabbitMQ.Models;

/// <summary>
/// Message model for robot status updates via RabbitMQ.
/// </summary>
public sealed record RobotStatusMessage
{
    public string MessageId { get; init; } = Guid.NewGuid().ToString();
    public string? CorrelationId { get; init; }
    public Guid RobotId { get; init; }
    public string RobotName { get; init; } = default!;
    public RobotStatus Status { get; init; }
    public decimal? BatteryLevel { get; init; }
    public string? CurrentTask { get; init; }
    public DateTime Timestamp { get; init; }
    public string? AdditionalInfo { get; init; }
    public List<SensorReading>? SensorReadings { get; init; }
    public List<string>? ErrorCodes { get; init; }
    public List<string>? Warnings { get; init; }
    public decimal? OperatingHours { get; init; }
    public string? FirmwareVersion { get; init; }
    public ProductionInfo? ProductionInfo { get; init; }
    public NetworkInfo? NetworkInfo { get; init; }

    public static RobotStatusMessage Create(string robotId, RobotStatus status)
    {
        return new RobotStatusMessage
        {
            RobotId = Guid.TryParse(robotId, out var id) ? id : Guid.Empty,
            RobotName = robotId,
            Status = status,
            Timestamp = DateTime.UtcNow
        };
    }
}

public sealed record SensorReading
{
    public string SensorId { get; init; } = default!;
    public string SensorType { get; init; } = default!;
    public decimal Value { get; init; }
    public string? Unit { get; init; }
    public DateTime ReadingTime { get; init; }
}

public sealed record ProductionInfo
{
    public string? RecipeId { get; init; }
    public string? BatchNumber { get; init; }
    public int? UnitsProduced { get; init; }
    public int? DefectCount { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? EstimatedCompletion { get; init; }
    public decimal? ProgressPercentage { get; init; }
    public string? CurrentStep { get; init; }
    public int? TotalSteps { get; init; }
    public int? CurrentStepNumber { get; init; }
}

public sealed record NetworkInfo
{
    public string? IpAddress { get; init; }
    public string? MacAddress { get; init; }
    public int? SignalStrength { get; init; }
    public bool IsConnected { get; init; }
    public string? ConnectionType { get; init; }
    public DateTime? LastCommunication { get; init; }
}
