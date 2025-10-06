using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

public abstract partial class Asset : BaseEntity, IAggregateRoot
{
    #region Core Properties

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string SerialNumber { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public string Manufacturer { get; private set; } = default!;
    public string AssetTag { get; private set; } = default!;
    public AssetType AssetType { get; protected set; }
    public AssetStatus Status { get; private set; } = AssetStatus.Active;
    public string Location { get; private set; } = default!;
    public AssetCriticality Criticality { get; private set; }

    #endregion

    #region Lifecycle Properties

    public DateTime PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public DateTime? InstallationDate { get; private set; }
    public DateTime? WarrantyExpiryDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    #endregion

    #region Operational Properties

    public int OperatingHours { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public string? Notes { get; private set; }
    public string? ImageUrl { get; private set; }
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

    protected Asset() { }

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

    #region Overrides

    public override string ToString()
    {
        return $"{AssetType}: {Name} ({SerialNumber})";
    }

    #endregion
}
