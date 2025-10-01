using EquipTrack.Domain.Enums;

namespace EquipTrack.RabbitMQ.Models;

/// <summary>
/// Message model for robot status updates via RabbitMQ.
/// </summary>
public sealed record RobotStatusMessage
{
    public Guid RobotId { get; init; }
    public string RobotName { get; init; } = default!;
    public RobotStatus Status { get; init; }
    public decimal? BatteryLevel { get; init; }
    public string? CurrentTask { get; init; }
    public DateTime Timestamp { get; init; }
    public string? AdditionalInfo { get; init; }
}
