using EquipTrack.Domain.Enums;

namespace EquipTrack.RabbitMQ.Models;

/// <summary>
/// Message model for machine status updates via RabbitMQ.
/// </summary>
public sealed record MachineStatusMessage
{
    public Guid MachineId { get; init; }
    public string MachineName { get; init; } = default!;
    public MachineStatus Status { get; init; }
    public DateTime Timestamp { get; init; }
    public string? AdditionalInfo { get; init; }
}
