using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a single sensor reading captured at a specific time.
/// Immutable record of sensor data for historical tracking.
/// </summary>
public class SensorReading : BaseEntity
{
    /// <summary>
    /// Foreign key to the sensor asset
    /// </summary>
    public Guid SensorRef { get; private set; }

    /// <summary>
    /// The measured value
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Unit of measurement (e.g., "°C", "bar", "rpm")
    /// </summary>
    public string Unit { get; private set; } = default!;

    /// <summary>
    /// Timestamp when the reading was taken
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Quality indicator for the reading
    /// </summary>
    public ReadingQuality Quality { get; private set; }

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public string? Metadata { get; private set; }

    /// <summary>
    /// Navigation property to the sensor asset
    /// </summary>
    public Asset Sensor { get; private set; } = default!;

    // EF Core constructor
    protected SensorReading() { }

    private SensorReading(
        Guid sensorId,
        decimal value,
        string unit,
        ReadingQuality quality = ReadingQuality.Good,
        string? metadata = null)
    {
        SensorRef = sensorId;
        Value = value;
        Unit = Ensure.NotEmpty(unit, nameof(unit));
        Timestamp = DateTime.UtcNow;
        Quality = quality;
        Metadata = metadata;
    }

    /// <summary>
    /// Factory method to create a new sensor reading
    /// </summary>
    public static SensorReading Create(
        Guid sensorId,
        decimal value,
        string unit,
        ReadingQuality quality = ReadingQuality.Good,
        string? metadata = null)
    {
        return new SensorReading(sensorId, value, unit, quality, metadata);
    }

    /// <summary>
    /// Updates the quality of the reading
    /// </summary>
    public void UpdateQuality(ReadingQuality newQuality)
    {
        Quality = newQuality;
    }

    public override string ToString()
    {
        return $"{Value} {Unit} at {Timestamp:yyyy-MM-dd HH:mm:ss} (Quality: {Quality})";
    }
}