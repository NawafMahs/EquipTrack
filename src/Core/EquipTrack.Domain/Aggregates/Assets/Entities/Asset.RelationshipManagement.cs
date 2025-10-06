using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Events;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Asset partial class - Relationship Management
/// Contains methods for managing relationships with other entities (logs, work orders, sensors, etc.)
/// </summary>
public abstract partial class Asset
{
    #region Relationship Management

    /// <summary>
    /// Adds a log entry
    /// </summary>
    public void AddLog(AssetLogType logType, string message, LogSeverity severity, string? source = null)
    {
        var log = AssetLog.Create(Id, logType, message, severity, source);
        _logs.Add(log);
    }

    /// <summary>
    /// Adds a work order
    /// </summary>
    public void AddWorkOrder(WorkOrder workOrder)
    {
        _workOrders.Add(Ensure.NotNull(workOrder, nameof(workOrder)));
    }

    /// <summary>
    /// Adds a preventive maintenance schedule
    /// </summary>
    public void AddPreventiveMaintenance(PreventiveMaintenance maintenance)
    {
        _preventiveMaintenances.Add(Ensure.NotNull(maintenance, nameof(maintenance)));
    }

    /// <summary>
    /// Attaches a sensor to the asset
    /// </summary>
    public void AttachSensor(Guid sensorId, string mountLocation)
    {
        var assetSensor = AssetSensor.Create(Id, sensorId, mountLocation);
        _sensors.Add(assetSensor);
        
        AddDomainEvent(new AssetSensorAttachedEvent(Id, sensorId, mountLocation));
        AddLog(AssetLogType.ConfigurationChanged, $"Sensor attached at {mountLocation}", LogSeverity.Info);
    }

    /// <summary>
    /// Records spare part usage
    /// </summary>
    public void RecordSparePartUsage(Guid sparePartId, int quantity, Guid? workOrderId = null, string? notes = null)
    {
        var usage = AssetSparePartUsage.Create(Id, sparePartId, quantity, workOrderId, notes);
        _sparePartUsages.Add(usage);
        
        AddDomainEvent(new AssetSparePartUsedEvent(Id, sparePartId, quantity, workOrderId, notes));
        AddLog(AssetLogType.MaintenancePerformed, $"Spare part used: Quantity {quantity}", LogSeverity.Info);
    }

    /// <summary>
    /// Records a sensor reading (for sensor-type assets)
    /// </summary>
    public virtual void RecordSensorReading(decimal value, string unit, ReadingQuality quality = ReadingQuality.Good)
    {
        if (AssetType != AssetType.Sensor)
            throw new InvalidOperationException("Only sensor-type assets can record readings.");

        var reading = SensorReading.Create(Id, value, unit, quality);
        _sensorReadings.Add(reading);
    }

    #endregion
}