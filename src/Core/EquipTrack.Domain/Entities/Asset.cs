using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;


public class Asset : BaseEntity, IAggregateRoot
{
    // Simple properties with private set for mutability
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string SerialNumber { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public string Manufacturer { get; private set; } = default!;
    public string AssetTag { get; private set; } = default!;
    public DateTime PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public AssetCriticality Criticality { get; private set; }
    public DateTime? InstallationDate { get; private set; }

    // Properties that can change internally
    public string Location { get; private set; } = default!;
    public AssetStatus Status { get; private set; } = AssetStatus.Active;
    public DateTime? WarrantyExpiryDate { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Notes { get; private set; }

    // Collections (internal, readonly for outside)
    private readonly List<WorkOrder> _workOrders = new();
    private readonly List<PreventiveMaintenance> _preventiveMaintenances = new();
    private readonly List<WorkOrderSparePart> _workOrderSpareParts = new();

    public IReadOnlyCollection<WorkOrder> WorkOrders => _workOrders.AsReadOnly();
    public IReadOnlyCollection<PreventiveMaintenance> PreventiveMaintenances => _preventiveMaintenances.AsReadOnly();
    public IReadOnlyCollection<WorkOrderSparePart> WorkOrderSpareParts => _workOrderSpareParts.AsReadOnly();

    // EF Core parameterless constructor
    protected Asset() { }
    protected Asset(
        string name,
        string description,
        string serialNumber,
        string model,
        string manufacturer,
        string location,
        DateTime purchaseDate,
        decimal purchasePrice,
        string assetTag = "",
        AssetCriticality criticality = AssetCriticality.Medium)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
        Description = Ensure.NotEmpty(description, nameof(description));
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
        Model = Ensure.NotEmpty(model, nameof(model));
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
        Location = Ensure.NotEmpty(location, nameof(location));
        PurchaseDate = purchaseDate;
        PurchasePrice = Ensure.Positive(purchasePrice, nameof(purchasePrice));
        AssetTag = assetTag;
        Criticality = criticality;
    }

    // Factory Method
    public static Asset Create(
    string name,
    string description,
    string serialNumber,
    string model,
    string manufacturer,
    string location,
    DateTime purchaseDate,
    decimal purchasePrice)
    {
        return new(name, description, serialNumber, model, manufacturer, location, purchaseDate, purchasePrice);
    }


    #region Behaviors

    public void SetName(string name)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
    }

    public void SetDescription(string description)
    {
        Description = Ensure.NotEmpty(description, nameof(description));
    }

    public void SetAssetTag(string assetTag)
    {
        AssetTag = assetTag ?? string.Empty;
    }

    public void SetSerialNumber(string serialNumber)
    {
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
    }

    public void SetManufacturer(string manufacturer)
    {
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
    }

    public void SetModel(string model)
    {
        Model = Ensure.NotEmpty(model, nameof(model));
    }

    public void SetCriticality(AssetCriticality criticality)
    {
        Criticality = criticality;
    }

    public void SetInstallationDate(DateTime? installationDate)
    {
        InstallationDate = installationDate;
    }

    public void SetWarrantyExpirationDate(DateTime? expirationDate)
    {
        if (expirationDate.HasValue && expirationDate.Value < PurchaseDate)
            throw new ArgumentException("Warranty expiry cannot be before purchase date.", nameof(expirationDate));
        WarrantyExpiryDate = expirationDate;
    }

    public void SetPurchaseInfo(DateTime? purchaseDate, decimal? purchasePrice)
    {
        if (purchaseDate.HasValue)
            PurchaseDate = purchaseDate.Value;
        if (purchasePrice.HasValue)
            PurchasePrice = purchasePrice.Value;
    }

    public void UpdateLocation(string newLocation)
    {
        Location = Ensure.NotEmpty(newLocation, nameof(newLocation));
    }

    public void ChangeStatus(AssetStatus newStatus)
    {
        if (!IsValidStatusTransition(newStatus))
            throw new InvalidOperationException($"Invalid status transition: {Status} → {newStatus}");
        Status = newStatus;
    }

    public void UpdateStatus(AssetStatus newStatus)
    {
        ChangeStatus(newStatus);
    }

    public void AddWorkOrder(WorkOrder workOrder)
    {
        _workOrders.Add(Ensure.NotNull(workOrder, nameof(workOrder)));
    }

    public void AddPreventiveMaintenance(PreventiveMaintenance maintenance)
    {
        _preventiveMaintenances.Add(Ensure.NotNull(maintenance, nameof(maintenance)));
    }

    public void AddNote(string note)
    {
        note = Ensure.NotEmpty(note, nameof(note));
        Notes = string.IsNullOrWhiteSpace(Notes) ? note : $"{Notes}\n{note}";
    }

    public void SetWarrantyExpiry(DateTime expiryDate)
    {
        if (expiryDate < PurchaseDate)
            throw new ArgumentException("Warranty expiry cannot be before purchase date.", nameof(expiryDate));
        WarrantyExpiryDate = expiryDate;
    }

    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = Ensure.NotEmpty(imageUrl, nameof(imageUrl));
    }
    
    public bool IsUnderWarranty() => WarrantyExpiryDate.HasValue && WarrantyExpiryDate.Value > DateTime.UtcNow;

    public bool IsOperational() => Status == AssetStatus.Active;


    #endregion

    #region Private Helpers

    private bool IsValidStatusTransition(AssetStatus newStatus) =>
        Status switch
        {
            AssetStatus.Active => newStatus is AssetStatus.UnderMaintenance or AssetStatus.OutOfService or AssetStatus.Disposed,
            AssetStatus.UnderMaintenance => newStatus is AssetStatus.Active or AssetStatus.OutOfService,
            AssetStatus.OutOfService => newStatus is AssetStatus.Active or AssetStatus.Disposed,
            AssetStatus.Inactive => newStatus is AssetStatus.Active,
            AssetStatus.Disposed => false,
            _ => false
        };

    #endregion
}
