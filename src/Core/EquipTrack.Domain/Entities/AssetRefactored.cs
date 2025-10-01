using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.ValueObjects;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents any maintainable asset in the CMMS system.
/// Base class for all equipment types using Table-Per-Hierarchy (TPH) inheritance.
/// Follows Domain-Driven Design principles with rich domain model.
/// </summary>
public abstract class Asset : BaseEntity, IAggregateRoot
{
    #region Core Properties

    /// <summary>
    /// Asset name for display and identification
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Detailed description of the asset
    /// </summary>
    public string Description { get; private set; } = default!;

    /// <summary>
    /// Unique serial number from manufacturer
    /// </summary>
    public string SerialNumber { get; private set; } = default!;

    /// <summary>
    /// Model number or designation
    /// </summary>
    public string Model { get; private set; } = default!;

    /// <summary>
    /// Manufacturer or vendor name
    /// </summary>
    public string Manufacturer { get; private set; } = default!;

    /// <summary>
    /// Internal asset tag for tracking
    /// </summary>
    public string AssetTag { get; private set; } = default!;

    /// <summary>
    /// Type discriminator for TPH inheritance
    /// </summary>
    public AssetType AssetType { get; protected set; }

    /// <summary>
    /// Current operational status
    /// </summary>
    public AssetStatus Status { get; private set; } = AssetStatus.Active;

    /// <summary>
    /// Physical location of the asset
    /// </summary>
    public string Location { get; private set; } = default!;

    /// <summary>
    /// Criticality level for maintenance prioritization
    /// </summary>
    public AssetCriticality Criticality { get; private set; }

    #endregion

    #region Lifecycle Properties

    /// <summary>
    /// Date when asset was purchased
    /// </summary>
    public DateTime PurchaseDate { get; private set; }

    /// <summary>
    /// Purchase price in base currency
    /// </summary>
    public decimal PurchasePrice { get; private set; }

    /// <summary>
    /// Date when asset was installed and commissioned
    /// </summary>
    public DateTime? InstallationDate { get; private set; }

    /// <summary>
    /// Warranty expiration date
    /// </summary>
    public DateTime? WarrantyExpiryDate { get; private set; }

    /// <summary>
    /// Whether the asset is currently active
    /// </summary>
    public bool IsActive { get; private set; } = true;

    #endregion

    #region Operational Properties

    /// <summary>
    /// Total operating hours accumulated
    /// </summary>
    public int OperatingHours { get; private set; }

    /// <summary>
    /// Date of last maintenance activity
    /// </summary>
    public DateTime? LastMaintenanceDate { get; private set; }

    /// <summary>
    /// Scheduled date for next maintenance
    /// </summary>
    public DateTime? NextMaintenanceDate { get; private set; }

    /// <summary>
    /// Additional notes and comments
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// URL to asset image or photo
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Extensible metadata for type-specific attributes
    /// Stored as JSON in database
    /// </summary>
    public Dictionary<string, string> Metadata { get; private set; } = new();

    #endregion

    #region Navigation Properties

    private readonly List<AssetLog> _logs = new();
    private readonly List<WorkOrder> _workOrders = new();
    private readonly List<PreventiveMaintenance> _preventiveMaintenances = new();
    private readonly List<AssetSensor> _sensors = new();
    private readonly List<AssetSparePartUsage> _sparePartUsages = new();
    private readonly List<SensorReading> _sensorReadings = new();

    public IReadOnlyCollection<AssetLog> Logs => _logs.AsReadOnly();
    public IReadOnlyCollection<WorkOrder> WorkOrders => _workOrders.AsReadOnly();
    public IReadOnlyCollection<PreventiveMaintenance> PreventiveMaintenances => _preventiveMaintenances.AsReadOnly();
    public IReadOnlyCollection<AssetSensor> Sensors => _sensors.AsReadOnly();
    public IReadOnlyCollection<AssetSparePartUsage> SparePartUsages => _sparePartUsages.AsReadOnly();
    public IReadOnlyCollection<SensorReading> SensorReadings => _sensorReadings.AsReadOnly();

    #endregion

    #region Constructors

    /// <summary>
    /// EF Core parameterless constructor
    /// </summary>
    protected Asset() { }

    /// <summary>
    /// Protected constructor for derived classes
    /// </summary>
    protected Asset(
        string name,
        string description,
        string serialNumber,
        string model,
        string manufacturer,
        string assetTag,
        AssetType assetType,
        string location,
        DateTime purchaseDate,
        decimal purchasePrice,
        AssetCriticality criticality = AssetCriticality.Medium)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
        Description = Ensure.NotEmpty(description, nameof(description));
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
        Model = Ensure.NotEmpty(model, nameof(model));
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
        AssetTag = Ensure.NotEmpty(assetTag, nameof(assetTag));
        AssetType = assetType;
        Location = Ensure.NotEmpty(location, nameof(location));
        PurchaseDate = purchaseDate;
        PurchasePrice = Ensure.Positive(purchasePrice, nameof(purchasePrice));
        Criticality = criticality;
    }

    #endregion

    #region Core Behaviors

    /// <summary>
    /// Updates the asset's name
    /// </summary>
    public void UpdateName(string name)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
        AddLog(AssetLogType.ConfigurationChanged, $"Name updated to: {name}", LogSeverity.Info);
    }

    /// <summary>
    /// Updates the asset's description
    /// </summary>
    public void UpdateDescription(string description)
    {
        Description = Ensure.NotEmpty(description, nameof(description));
    }

    /// <summary>
    /// Updates the asset's location
    /// </summary>
    public virtual void UpdateLocation(string newLocation)
    {
        var oldLocation = Location;
        Location = Ensure.NotEmpty(newLocation, nameof(newLocation));
        AddLog(AssetLogType.ConfigurationChanged, $"Location changed from '{oldLocation}' to '{newLocation}'", LogSeverity.Info);
    }

    /// <summary>
    /// Changes the asset's operational status with validation
    /// </summary>
    public virtual void ChangeStatus(AssetStatus newStatus)
    {
        if (!IsValidStatusTransition(newStatus))
            throw new InvalidOperationException($"Invalid status transition: {Status} → {newStatus}");

        var oldStatus = Status;
        Status = newStatus;
        AddLog(AssetLogType.StatusChange, $"Status changed from {oldStatus} to {newStatus}", LogSeverity.Info);
    }

    /// <summary>
    /// Updates the asset's criticality level
    /// </summary>
    public void UpdateCriticality(AssetCriticality criticality)
    {
        Criticality = criticality;
        AddLog(AssetLogType.ConfigurationChanged, $"Criticality updated to: {criticality}", LogSeverity.Info);
    }

    /// <summary>
    /// Sets the installation date
    /// </summary>
    public void SetInstallationDate(DateTime installationDate)
    {
        if (installationDate < PurchaseDate)
            throw new ArgumentException("Installation date cannot be before purchase date.", nameof(installationDate));

        InstallationDate = installationDate;
    }

    /// <summary>
    /// Sets the warranty expiry date
    /// </summary>
    public void SetWarrantyExpiry(DateTime expiryDate)
    {
        if (expiryDate < PurchaseDate)
            throw new ArgumentException("Warranty expiry cannot be before purchase date.", nameof(expiryDate));

        WarrantyExpiryDate = expiryDate;
    }

    /// <summary>
    /// Updates purchase information
    /// </summary>
    public void UpdatePurchaseInfo(DateTime? purchaseDate, decimal? purchasePrice)
    {
        if (purchaseDate.HasValue)
            PurchaseDate = purchaseDate.Value;
        
        if (purchasePrice.HasValue)
            PurchasePrice = Ensure.Positive(purchasePrice.Value, nameof(purchasePrice));
    }

    /// <summary>
    /// Sets the asset's image URL
    /// </summary>
    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = Ensure.NotEmpty(imageUrl, nameof(imageUrl));
    }

    /// <summary>
    /// Adds a note to the asset
    /// </summary>
    public void AddNote(string note)
    {
        note = Ensure.NotEmpty(note, nameof(note));
        Notes = string.IsNullOrWhiteSpace(Notes) 
            ? note 
            : $"{Notes}\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: {note}";
        
        AddLog(AssetLogType.ManualEntry, $"Note added: {note}", LogSeverity.Info);
    }

    /// <summary>
    /// Adds or updates metadata
    /// </summary>
    public void SetMetadata(string key, string value)
    {
        Ensure.NotEmpty(key, nameof(key));
        Metadata[key] = value;
    }

    /// <summary>
    /// Gets metadata value by key
    /// </summary>
    public string? GetMetadata(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }

    #endregion

    #region Maintenance Behaviors

    /// <summary>
    /// Records maintenance activity
    /// </summary>
    public virtual void RecordMaintenance(DateTime maintenanceDate, DateTime? nextScheduledDate = null, string? notes = null)
    {
        LastMaintenanceDate = maintenanceDate;
        NextMaintenanceDate = nextScheduledDate;

        var message = $"Maintenance performed on {maintenanceDate:yyyy-MM-dd}";
        if (nextScheduledDate.HasValue)
            message += $". Next scheduled: {nextScheduledDate.Value:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(notes))
            message += $". Notes: {notes}";

        AddLog(AssetLogType.MaintenancePerformed, message, LogSeverity.Info);
    }

    /// <summary>
    /// Increments operating hours
    /// </summary>
    public virtual void IncrementOperatingHours(int hours)
    {
        if (hours < 0)
            throw new ArgumentException("Operating hours cannot be negative.", nameof(hours));

        OperatingHours += hours;
    }

    /// <summary>
    /// Checks if asset requires maintenance
    /// </summary>
    public bool RequiresMaintenance()
    {
        return NextMaintenanceDate.HasValue && NextMaintenanceDate.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if asset is under warranty
    /// </summary>
    public bool IsUnderWarranty()
    {
        return WarrantyExpiryDate.HasValue && WarrantyExpiryDate.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if asset is operational
    /// </summary>
    public bool IsOperational()
    {
        return IsActive && (Status == AssetStatus.Active || Status == AssetStatus.Running || Status == AssetStatus.Idle);
    }

    #endregion

    #region Lifecycle Behaviors

    /// <summary>
    /// Activates the asset
    /// </summary>
    public virtual void Activate()
    {
        IsActive = true;
        Status = AssetStatus.Active;
        AddLog(AssetLogType.StatusChange, "Asset activated", LogSeverity.Info);
    }

    /// <summary>
    /// Deactivates the asset
    /// </summary>
    public virtual void Deactivate()
    {
        IsActive = false;
        Status = AssetStatus.Inactive;
        AddLog(AssetLogType.StatusChange, "Asset deactivated", LogSeverity.Warning);
    }

    /// <summary>
    /// Marks asset as disposed
    /// </summary>
    public void Dispose(string reason)
    {
        IsActive = false;
        Status = AssetStatus.Disposed;
        AddNote($"Asset disposed. Reason: {reason}");
        AddLog(AssetLogType.StatusChange, $"Asset disposed: {reason}", LogSeverity.Warning);
    }

    #endregion

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
        AddLog(AssetLogType.ConfigurationChanged, $"Sensor attached at {mountLocation}", LogSeverity.Info);
    }

    /// <summary>
    /// Records spare part usage
    /// </summary>
    public void RecordSparePartUsage(Guid sparePartId, int quantity, Guid? workOrderId = null, string? notes = null)
    {
        var usage = AssetSparePartUsage.Create(Id, sparePartId, quantity, workOrderId, notes);
        _sparePartUsages.Add(usage);
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

    #region Private Helpers

    /// <summary>
    /// Validates status transitions based on current state
    /// </summary>
    protected virtual bool IsValidStatusTransition(AssetStatus newStatus)
    {
        return Status switch
        {
            AssetStatus.Active => newStatus is AssetStatus.Running or AssetStatus.Idle or AssetStatus.UnderMaintenance or AssetStatus.OutOfService or AssetStatus.Inactive,
            AssetStatus.Idle => newStatus is AssetStatus.Running or AssetStatus.Active or AssetStatus.UnderMaintenance or AssetStatus.OutOfService,
            AssetStatus.Running => newStatus is AssetStatus.Idle or AssetStatus.Active or AssetStatus.Error or AssetStatus.UnderMaintenance,
            AssetStatus.UnderMaintenance => newStatus is AssetStatus.Active or AssetStatus.Idle or AssetStatus.OutOfService,
            AssetStatus.Error => newStatus is AssetStatus.UnderMaintenance or AssetStatus.OutOfService,
            AssetStatus.OutOfService => newStatus is AssetStatus.UnderMaintenance or AssetStatus.Disposed,
            AssetStatus.Charging => newStatus is AssetStatus.Idle or AssetStatus.Running or AssetStatus.Active,
            AssetStatus.Inactive => newStatus is AssetStatus.Active or AssetStatus.Disposed,
            AssetStatus.Disposed => false,
            _ => false
        };
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return $"{AssetType}: {Name} ({SerialNumber})";
    }

    #endregion
}
