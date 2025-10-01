using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a physical machine in the CMMS system.
/// Tracks lifecycle, maintenance, and operational data.
/// </summary>
public class Machine : BaseEntity, IAggregateRoot
{
    // Immutable properties
    public string Name { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public string Model { get; init; } = default!;
    public string Manufacturer { get; init; } = default!;
    public DateTime InstallationDate { get; init; }
    public string MachineTypeRef { get; init; } = default!;

    // Mutable properties
    public string Location { get; private set; } = default!;
    public MachineStatus Status { get; private set; } = MachineStatus.Idle;
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public int OperatingHours { get; private set; }
    public decimal? CurrentEfficiency { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    private readonly List<MachineSensor> _sensors = new();
    private readonly List<MachineLog> _logs = new();
    private readonly List<MachineSparePartUsage> _sparePartUsages = new();
    private readonly List<WorkOrder> _workOrders = new();

    public IReadOnlyCollection<MachineSensor> Sensors => _sensors.AsReadOnly();
    public IReadOnlyCollection<MachineLog> Logs => _logs.AsReadOnly();
    public IReadOnlyCollection<MachineSparePartUsage> SparePartUsages => _sparePartUsages.AsReadOnly();
    public IReadOnlyCollection<WorkOrder> WorkOrders => _workOrders.AsReadOnly();

    // EF Core constructor
    protected Machine() { }

    private Machine(
        string name,
        string serialNumber,
        string model,
        string manufacturer,
        string location,
        DateTime installationDate,
        string machineTypeRef)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
        Model = Ensure.NotEmpty(model, nameof(model));
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
        Location = Ensure.NotEmpty(location, nameof(location));
        InstallationDate = installationDate;
        MachineTypeRef = Ensure.NotEmpty(machineTypeRef, nameof(machineTypeRef));
    }

    // Factory method
    public static Machine Create(
        string name,
        string serialNumber,
        string model,
        string manufacturer,
        string location,
        DateTime installationDate,
        string machineTypeRef)
    {
        return new Machine(name, serialNumber, model, manufacturer, location, installationDate, machineTypeRef);
    }

    #region Behaviors

    public void UpdateLocation(string newLocation)
    {
        Location = Ensure.NotEmpty(newLocation, nameof(newLocation));
    }

    public void ChangeStatus(MachineStatus newStatus)
    {
        if (!IsValidStatusTransition(newStatus))
            throw new InvalidOperationException($"Invalid status transition: {Status} → {newStatus}");
        
        Status = newStatus;
        LogStatusChange(newStatus);
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

    public void UpdateEfficiency(decimal efficiency)
    {
        if (efficiency < 0 || efficiency > 100)
            throw new ArgumentException("Efficiency must be between 0 and 100.", nameof(efficiency));
        
        CurrentEfficiency = efficiency;
    }

    public void AddSensor(MachineSensor sensor)
    {
        _sensors.Add(Ensure.NotNull(sensor, nameof(sensor)));
    }

    public void AddLog(MachineLog log)
    {
        _logs.Add(Ensure.NotNull(log, nameof(log)));
    }

    public void RecordSparePartUsage(MachineSparePartUsage usage)
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
        Status = MachineStatus.OutOfService;
    }

    public void Activate()
    {
        IsActive = true;
        Status = MachineStatus.Idle;
    }

    public bool RequiresMaintenance()
    {
        return NextMaintenanceDate.HasValue && NextMaintenanceDate.Value <= DateTime.UtcNow;
    }

    #endregion

    #region Private Helpers

    private bool IsValidStatusTransition(MachineStatus newStatus) =>
        Status switch
        {
            MachineStatus.Idle => newStatus is MachineStatus.Running or MachineStatus.Maintenance or MachineStatus.OutOfService,
            MachineStatus.Running => newStatus is MachineStatus.Idle or MachineStatus.Error or MachineStatus.Maintenance,
            MachineStatus.Error => newStatus is MachineStatus.Maintenance or MachineStatus.OutOfService,
            MachineStatus.Maintenance => newStatus is MachineStatus.Idle or MachineStatus.Running,
            MachineStatus.OutOfService => newStatus is MachineStatus.Maintenance,
            _ => false
        };

    private void LogStatusChange(MachineStatus newStatus)
    {
        var log = MachineLog.Create(
            Id,
            MachineLogType.StatusChange,
            $"Status changed from {Status} to {newStatus}",
            MachineLogSeverity.Info
        );
        AddLog(log);
    }

    #endregion
}
