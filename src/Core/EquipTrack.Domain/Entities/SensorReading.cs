using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a single sensor reading captured at a specific time.
/// Immutable record of sensor data for historical tracking.
/// </summary>
public class SensorReading : BaseEntity
{
    public Guid SensorRef { get; init; }
    public decimal Value { get; init; }
    public DateTime Timestamp { get; init; }
    public string? Metadata { get; init; }

    // EF Core constructor
    protected SensorReading() { }

    private SensorReading(Guid sensorRef, decimal value, DateTime timestamp, string? metadata = null)
    {
        SensorRef = sensorRef;
        Value = value;
        Timestamp = timestamp;
        Metadata = metadata;
    }

    public static SensorReading Create(Guid sensorRef, decimal value, DateTime timestamp, string? metadata = null)
    {
        return new SensorReading(sensorRef, value, timestamp, metadata);
    }
}
