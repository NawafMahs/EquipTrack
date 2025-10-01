using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a sensor attached to a machine for monitoring.
/// </summary>
public class MachineSensor : BaseEntity
{
    public Guid MachineRef { get; init; }
    public string SensorName { get; init; } = default!;
    public SensorType SensorType { get; init; }
    public string Unit { get; init; } = default!;
    public decimal? MinThreshold { get; private set; }
    public decimal? MaxThreshold { get; private set; }
    public decimal? CurrentValue { get; private set; }
    public DateTime? LastReadingTime { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int ReadingIntervalSeconds { get; private set; } = 60;

    // Navigation
    private readonly List<SensorReading> _readings = new();
    public IReadOnlyCollection<SensorReading> Readings => _readings.AsReadOnly();

    // EF Core constructor
    protected MachineSensor() { }

    private MachineSensor(
        Guid machineRef,
        string sensorName,
        SensorType sensorType,
        string unit,
        int readingIntervalSeconds)
    {
        MachineRef = machineRef;
        SensorName = Ensure.NotEmpty(sensorName, nameof(sensorName));
        SensorType = sensorType;
        Unit = Ensure.NotEmpty(unit, nameof(unit));
        ReadingIntervalSeconds = Ensure.Positive(readingIntervalSeconds, nameof(readingIntervalSeconds));
    }

    public static MachineSensor Create(
        Guid machineRef,
        string sensorName,
        SensorType sensorType,
        string unit,
        int readingIntervalSeconds = 60)
    {
        return new MachineSensor(machineRef, sensorName, sensorType, unit, readingIntervalSeconds);
    }

    #region Behaviors

    public void SetThresholds(decimal? minThreshold, decimal? maxThreshold)
    {
        if (minThreshold.HasValue && maxThreshold.HasValue && minThreshold >= maxThreshold)
            throw new ArgumentException("Min threshold must be less than max threshold.");
        
        MinThreshold = minThreshold;
        MaxThreshold = maxThreshold;
    }

    public void RecordReading(decimal value, DateTime timestamp)
    {
        CurrentValue = value;
        LastReadingTime = timestamp;

        var reading = SensorReading.Create(Id, value, timestamp);
        _readings.Add(reading);

        CheckThresholds(value);
    }

    public void UpdateReadingInterval(int intervalSeconds)
    {
        ReadingIntervalSeconds = Ensure.Positive(intervalSeconds, nameof(intervalSeconds));
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public bool IsOutOfThreshold()
    {
        if (!CurrentValue.HasValue) return false;
        
        if (MinThreshold.HasValue && CurrentValue < MinThreshold) return true;
        if (MaxThreshold.HasValue && CurrentValue > MaxThreshold) return true;
        
        return false;
    }

    #endregion

    #region Private Helpers

    private void CheckThresholds(decimal value)
    {
        if (MinThreshold.HasValue && value < MinThreshold)
        {
            // Raise domain event for threshold breach
            AddDomainEvent(new SensorThresholdBreachedEvent(Id, MachineRef, SensorName, value, MinThreshold.Value, "Below"));
        }
        
        if (MaxThreshold.HasValue && value > MaxThreshold)
        {
            // Raise domain event for threshold breach
            AddDomainEvent(new SensorThresholdBreachedEvent(Id, MachineRef, SensorName, value, MaxThreshold.Value, "Above"));
        }
    }

    #endregion
}

/// <summary>
/// Domain event raised when a sensor reading breaches a threshold.
/// </summary>
public record SensorThresholdBreachedEvent(
    Guid SensorId,
    Guid MachineRef,
    string SensorName,
    decimal CurrentValue,
    decimal ThresholdValue,
    string BreachType) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
