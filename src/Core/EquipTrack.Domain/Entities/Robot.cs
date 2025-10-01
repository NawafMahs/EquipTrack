using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents an autonomous robot in the CMMS system.
/// Extends machine capabilities with robot-specific features.
/// </summary>
public class Robot : BaseEntity, IAggregateRoot
{
    // Immutable properties
    public string Name { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public string Model { get; init; } = default!;
    public string Manufacturer { get; init; } = default!;
    public DateTime InstallationDate { get; init; }
    public RobotType RobotType { get; init; }
    public int MaxPayloadKg { get; init; }
    public decimal ReachMeters { get; init; }

    // Mutable properties
    public string Location { get; private set; } = default!;
    public RobotStatus Status { get; private set; } = RobotStatus.Idle;
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public int OperatingHours { get; private set; }
    public int CycleCount { get; private set; }
    public decimal? BatteryLevel { get; private set; }
    public string? CurrentTask { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    private readonly List<RobotSensor> _sensors = new();
    private readonly List<RobotLog> _logs = new();
    private readonly List<RobotSparePartUsage> _sparePartUsages = new();
    private readonly List<WorkOrder> _workOrders = new();

    public IReadOnlyCollection<RobotSensor> Sensors => _sensors.AsReadOnly();
    public IReadOnlyCollection<RobotLog> Logs => _logs.AsReadOnly();
    public IReadOnlyCollection<RobotSparePartUsage> SparePartUsages => _sparePartUsages.AsReadOnly();
    public IReadOnlyCollection<WorkOrder> WorkOrders => _workOrders.AsReadOnly();

    // EF Core constructor
    protected Robot() { }

    private Robot(
        string name,
        string serialNumber,
        string model,
        string manufacturer,
        string location,
        DateTime installationDate,
        RobotType robotType,
        int maxPayloadKg,
        decimal reachMeters)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
        Model = Ensure.NotEmpty(model, nameof(model));
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
        Location = Ensure.NotEmpty(location, nameof(location));
        InstallationDate = installationDate;
        RobotType = robotType;
        MaxPayloadKg = Ensure.Positive(maxPayloadKg, nameof(maxPayloadKg));
        ReachMeters = Ensure.Positive(reachMeters, nameof(reachMeters));
    }

    // Factory method
    public static Robot Create(
        string name,
        string serialNumber,
        string model,
        string manufacturer,
        string location,
        DateTime installationDate,
        RobotType robotType,
        int maxPayloadKg,
        decimal reachMeters)
    {
        return new Robot(name, serialNumber, model, manufacturer, location, installationDate, robotType, maxPayloadKg, reachMeters);
    }

    #region Behaviors

    public void UpdateLocation(string newLocation)
    {
        Location = Ensure.NotEmpty(newLocation, nameof(newLocation));
    }

    public void ChangeStatus(RobotStatus newStatus)
    {
        if (!IsValidStatusTransition(newStatus))
            throw new InvalidOperationException($"Invalid status transition: {Status} → {newStatus}");
        
        Status = newStatus;
        LogStatusChange(newStatus);
    }

    public void AssignTask(string task)
    {
        CurrentTask = Ensure.NotEmpty(task, nameof(task));
        if (Status == RobotStatus.Idle)
            ChangeStatus(RobotStatus.Running);
    }

    public void CompleteTask()
    {
        CurrentTask = null;
        CycleCount++;
        if (Status == RobotStatus.Running)
            ChangeStatus(RobotStatus.Idle);
    }

    public void UpdateBatteryLevel(decimal level)
    {
        if (level < 0 || level > 100)
            throw new ArgumentException("Battery level must be between 0 and 100.", nameof(level));
        
        BatteryLevel = level;
        
        if (level < 20)
            LogLowBattery(level);
    }

    public void RecordMaintenance(DateTime maintenanceDate, DateTime? nextScheduledDate = null)
    {
        LastMaintenanceDate = maintenanceDate;
        NextMaintenanceDate = nextScheduledDate;
    }

    public void IncrementOperatingHours(int hours)
    {
        if (hours < 0)
            throw new ArgumentException("Operating hours cannot be negative.", nameof(hours));
        
        OperatingHours += hours;
    }

    public void AddSensor(RobotSensor sensor)
    {
        _sensors.Add(Ensure.NotNull(sensor, nameof(sensor)));
    }

    public void AddLog(RobotLog log)
    {
        _logs.Add(Ensure.NotNull(log, nameof(log)));
    }

    public void RecordSparePartUsage(RobotSparePartUsage usage)
    {
        _sparePartUsages.Add(Ensure.NotNull(usage, nameof(usage)));
    }

    public void AddWorkOrder(WorkOrder workOrder)
    {
        _workOrders.Add(Ensure.NotNull(workOrder, nameof(workOrder)));
    }

    public void AddNote(string note)
    {
        note = Ensure.NotEmpty(note, nameof(note));
        Notes = string.IsNullOrWhiteSpace(Notes) ? note : $"{Notes}\n{note}";
    }

    public void Deactivate()
    {
        IsActive = false;
        Status = RobotStatus.OutOfService;
        CurrentTask = null;
    }

    public void Activate()
    {
        IsActive = true;
        Status = RobotStatus.Idle;
    }

    public bool RequiresMaintenance()
    {
        return NextMaintenanceDate.HasValue && NextMaintenanceDate.Value <= DateTime.UtcNow;
    }

    public bool RequiresCharging()
    {
        return BatteryLevel.HasValue && BatteryLevel.Value < 20;
    }

    #endregion

    #region Private Helpers

    private bool IsValidStatusTransition(RobotStatus newStatus) =>
        Status switch
        {
            RobotStatus.Idle => newStatus is RobotStatus.Running or RobotStatus.Charging or RobotStatus.Maintenance or RobotStatus.OutOfService,
            RobotStatus.Running => newStatus is RobotStatus.Idle or RobotStatus.Error or RobotStatus.Charging or RobotStatus.Maintenance,
            RobotStatus.Charging => newStatus is RobotStatus.Idle or RobotStatus.Running,
            RobotStatus.Error => newStatus is RobotStatus.Maintenance or RobotStatus.OutOfService,
            RobotStatus.Maintenance => newStatus is RobotStatus.Idle or RobotStatus.Running,
            RobotStatus.OutOfService => newStatus is RobotStatus.Maintenance,
            _ => false
        };

    private void LogStatusChange(RobotStatus newStatus)
    {
        var log = RobotLog.Create(
            Id,
            RobotLogType.StatusChange,
            $"Status changed from {Status} to {newStatus}",
            RobotLogSeverity.Info
        );
        AddLog(log);
    }

    private void LogLowBattery(decimal level)
    {
        var log = RobotLog.Create(
            Id,
            RobotLogType.BatteryAlert,
            $"Low battery level: {level}%",
            RobotLogSeverity.Warning
        );
        AddLog(log);
    }

    #endregion
}
