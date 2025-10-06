using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Events;

/// <summary>
/// Domain event raised when a sensor reading breaches a threshold.
/// Applies to all asset types (machines, robots, sensors, etc.).
/// </summary>
public class SensorThresholdBreachedEvent(
    Guid SensorId,
    Guid AssetId,
    string SensorName,
    decimal CurrentValue,
    decimal ThresholdValue,
    string BreachType) : BaseEvent
{
}