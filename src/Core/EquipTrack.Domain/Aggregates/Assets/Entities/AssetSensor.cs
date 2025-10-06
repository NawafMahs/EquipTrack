using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a sensor attached to an asset.
/// Unified entity replacing MachineSensor and RobotSensor.
/// Many-to-many relationship between Assets and Sensors.
/// </summary>
public class AssetSensor : BaseEntity
{
    /// <summary>
    /// Foreign key to the asset (machine, robot, etc.)
    /// </summary>
    public Guid AssetRef { get; private set; }

    /// <summary>
    /// Foreign key to the sensor (which is also an Asset with AssetType = Sensor)
    /// </summary>
    public Guid SensorRef { get; private set; }

    /// <summary>
    /// Physical location where the sensor is mounted
    /// </summary>
    public string MountLocation { get; private set; } = default!;

    /// <summary>
    /// Whether the sensor is currently active
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Date when the sensor was attached
    /// </summary>
    public DateTime AttachedDate { get; private set; }

    /// <summary>
    /// Date when the sensor was detached (if applicable)
    /// </summary>
    public DateTime? DetachedDate { get; private set; }

    /// <summary>
    /// Navigation property to the asset
    /// </summary>
    public Asset Asset { get; private set; } = default!;

    /// <summary>
    /// Navigation property to the sensor
    /// </summary>
    public Asset Sensor { get; private set; } = default!;

    // EF Core constructor
    protected AssetSensor() { }

    private AssetSensor(
        Guid assetId,
        Guid sensorId,
        string mountLocation)
    {
        AssetRef = assetId;
        SensorRef = sensorId;
        MountLocation = Ensure.NotEmpty(mountLocation, nameof(mountLocation));
        AttachedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new asset-sensor relationship
    /// </summary>
    public static AssetSensor Create(
        Guid assetId,
        Guid sensorId,
        string mountLocation)
    {
        return new AssetSensor(assetId, sensorId, mountLocation);
    }

    /// <summary>
    /// Activates the sensor
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        DetachedDate = null;
    }

    /// <summary>
    /// Deactivates the sensor
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        DetachedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the mount location
    /// </summary>
    public void UpdateMountLocation(string newLocation)
    {
        MountLocation = Ensure.NotEmpty(newLocation, nameof(newLocation));
    }

    public override string ToString()
    {
        return $"Sensor at {MountLocation} - Active: {IsActive}";
    }
}