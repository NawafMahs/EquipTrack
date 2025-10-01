using EquipTrack.Domain.Enums;

namespace EquipTrack.RabbitMQ.Models;

/// <summary>
/// Message model for sensor readings via RabbitMQ.
/// </summary>
public sealed record SensorReadingMessage
{
    public Guid SensorId { get; init; }
    public Guid? MachineId { get; init; }
    public Guid? RobotId { get; init; }
    public string SensorName { get; init; } = default!;
    public SensorType SensorType { get; init; }
    public decimal Value { get; init; }
    public string Unit { get; init; } = default!;
    public DateTime Timestamp { get; init; }
    public bool IsThresholdBreached { get; init; }
}
