using EquipTrack.Domain.Assets.Enums;
using EquipTrack.Domain.Assets.Events;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.WorkOrders.Entities;

namespace EquipTrack.Domain.Assets.Entities;

/// <summary>
/// Represents a physical or logical asset that requires maintenance.
/// </summary>
public class Asset : AggregateRoot
{
    private readonly List<WorkOrder> _workOrders = new();

    /// <summary>
    /// Gets the asset name.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the asset description.
    /// </summary>
    public string Description { get; private set; } = default!;

    /// <summary>
    /// Gets the asset tag or identifier.
    /// </summary>
    public string AssetTag { get; private set; } = default!;

    /// <summary>
    /// Gets the asset serial number.
    /// </summary>
    public string SerialNumber { get; private set; } = default!;

    /// <summary>
    /// Gets the manufacturer name.
    /// </summary>
    public string Manufacturer { get; private set; } = default!;

    /// <summary>
    /// Gets the model number.
    /// </summary>
    public string Model { get; private set; } = default!;

    /// <summary>
    /// Gets the location where the asset is installed.
    /// </summary>
    public string Location { get; private set; } = default!;

    /// <summary>
    /// Gets the current status of the asset.
    /// </summary>
    public AssetStatus Status { get; private set; }

    /// <summary>
    /// Gets the criticality level of the asset.
    /// </summary>
    public AssetCriticality Criticality { get; private set; }

    /// <summary>
    /// Gets the date when the asset was purchased.
    /// </summary>
    public DateTime? PurchaseDate { get; private set; }

    /// <summary>
    /// Gets the purchase cost of the asset.
    /// </summary>
    public decimal? PurchaseCost { get; private set; }

    /// <summary>
    /// Gets the date when the asset was put into service.
    /// </summary>
    public DateTime? InstallationDate { get; private set; }

    /// <summary>
    /// Gets the warranty expiration date.
    /// </summary>
    public DateTime? WarrantyExpirationDate { get; private set; }

    
    /// <summary>
    /// Gets the read-only collection of work orders associated with this asset.
    /// </summary>
    public IReadOnlyCollection<WorkOrder> WorkOrders => _workOrders.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Asset"/> class.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <param name="description">The asset description.</param>
    /// <param name="assetTag">The asset tag or identifier.</param>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="manufacturer">The manufacturer name.</param>
    /// <param name="model">The model number.</param>
    /// <param name="location">The installation location.</param>
    /// <param name="criticality">The criticality level.</param>
    private Asset(
        string name,
        string description,
        string assetTag,
        string serialNumber,
        string manufacturer,
        string model,
        string location,
        AssetCriticality criticality)
    {
        SetName(name);
        SetDescription(description);
        SetAssetTag(assetTag);
        SetSerialNumber(serialNumber);
        SetManufacturer(manufacturer);
        SetModel(model);
        SetLocation(location);
        SetCriticality(criticality);

        Status = AssetStatus.Operational;

        AddDomainEvent(new AssetCreatedEvent(Id, name, assetTag));
    }

    /// <summary>
    /// Protected parameterless constructor for ORM materialization.
    /// </summary>
    protected Asset() { }

    /// <summary>
    /// Creates a new asset with the specified details.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <param name="description">The asset description.</param>
    /// <param name="assetTag">The asset tag or identifier.</param>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="manufacturer">The manufacturer name.</param>
    /// <param name="model">The model number.</param>
    /// <param name="location">The installation location.</param>
    /// <param name="criticality">The criticality level.</param>
    /// <returns>A new Asset instance.</returns>
    /// <exception cref="ArgumentException">Thrown when inputs are invalid.</exception>
    public static Asset Create(
        string name,
        string description,
        string assetTag,
        string serialNumber,
        string manufacturer,
        string model,
        string location,
        AssetCriticality criticality)
    {
        ValidateAssetData(name, assetTag, serialNumber, manufacturer, model, location);

        return new Asset(
            name.Trim(),
            description?.Trim() ?? string.Empty,
            assetTag.Trim(),
            serialNumber.Trim(),
            manufacturer.Trim(),
            model.Trim(),
            location.Trim(),
            criticality);
    }

    /// <summary>
    /// Validates asset data for creation or updates.
    /// </summary>
    private static void ValidateAssetData(string name, string assetTag, string serialNumber, 
        string manufacturer, string model, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name must not be empty.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Asset name cannot exceed 200 characters.", nameof(name));

        if (string.IsNullOrWhiteSpace(assetTag))
            throw new ArgumentException("Asset tag must not be empty.", nameof(assetTag));

        if (assetTag.Length > 50)
            throw new ArgumentException("Asset tag cannot exceed 50 characters.", nameof(assetTag));

        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentException("Serial number must not be empty.", nameof(serialNumber));

        if (serialNumber.Length > 100)
            throw new ArgumentException("Serial number cannot exceed 100 characters.", nameof(serialNumber));

        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new ArgumentException("Manufacturer must not be empty.", nameof(manufacturer));

        if (manufacturer.Length > 100)
            throw new ArgumentException("Manufacturer cannot exceed 100 characters.", nameof(manufacturer));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model must not be empty.", nameof(model));

        if (model.Length > 100)
            throw new ArgumentException("Model cannot exceed 100 characters.", nameof(model));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location must not be empty.", nameof(location));

        if (location.Length > 200)
            throw new ArgumentException("Location cannot exceed 200 characters.", nameof(location));
    }

    /// <summary>
    /// Updates the asset status.
    /// </summary>
    /// <param name="newStatus">The new status.</param>
    public void UpdateStatus(AssetStatus newStatus)
    {
        if (Status == newStatus) return;

        var previousStatus = Status;
        Status = newStatus;
        MarkAsUpdated();

        AddDomainEvent(new AssetStatusChangedEvent(Id, previousStatus, newStatus));
    }

    /// <summary>
    /// Updates the asset name.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <exception cref="ArgumentException">Thrown when name is invalid.</exception>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name must not be empty.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Asset name cannot exceed 200 characters.", nameof(name));

        Name = name.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the asset description.
    /// </summary>
    /// <param name="description">The new description.</param>
    public void SetDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the asset tag.
    /// </summary>
    /// <param name="assetTag">The new asset tag.</param>
    /// <exception cref="ArgumentException">Thrown when asset tag is invalid.</exception>
    public void SetAssetTag(string assetTag)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new ArgumentException("Asset tag must not be empty.", nameof(assetTag));

        if (assetTag.Length > 50)
            throw new ArgumentException("Asset tag cannot exceed 50 characters.", nameof(assetTag));

        AssetTag = assetTag.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the serial number.
    /// </summary>
    /// <param name="serialNumber">The new serial number.</param>
    /// <exception cref="ArgumentException">Thrown when serial number is invalid.</exception>
    public void SetSerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentException("Serial number must not be empty.", nameof(serialNumber));

        if (serialNumber.Length > 100)
            throw new ArgumentException("Serial number cannot exceed 100 characters.", nameof(serialNumber));

        SerialNumber = serialNumber.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the manufacturer.
    /// </summary>
    /// <param name="manufacturer">The new manufacturer.</param>
    /// <exception cref="ArgumentException">Thrown when manufacturer is invalid.</exception>
    public void SetManufacturer(string manufacturer)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new ArgumentException("Manufacturer must not be empty.", nameof(manufacturer));

        if (manufacturer.Length > 100)
            throw new ArgumentException("Manufacturer cannot exceed 100 characters.", nameof(manufacturer));

        Manufacturer = manufacturer.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the model.
    /// </summary>
    /// <param name="model">The new model.</param>
    /// <exception cref="ArgumentException">Thrown when model is invalid.</exception>
    public void SetModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model must not be empty.", nameof(model));

        if (model.Length > 100)
            throw new ArgumentException("Model cannot exceed 100 characters.", nameof(model));

        Model = model.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the location.
    /// </summary>
    /// <param name="location">The new location.</param>
    /// <exception cref="ArgumentException">Thrown when location is invalid.</exception>
    public void SetLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location must not be empty.", nameof(location));

        if (location.Length > 200)
            throw new ArgumentException("Location cannot exceed 200 characters.", nameof(location));

        Location = location.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the criticality level.
    /// </summary>
    /// <param name="criticality">The new criticality level.</param>
    public void SetCriticality(AssetCriticality criticality)
    {
        Criticality = criticality;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the purchase information.
    /// </summary>
    /// <param name="purchaseDate">The purchase date.</param>
    /// <param name="purchaseCost">The purchase cost.</param>
    public void SetPurchaseInfo(DateTime? purchaseDate, decimal? purchaseCost)
    {
        if (purchaseCost.HasValue && purchaseCost.Value < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        PurchaseDate = purchaseDate;
        PurchaseCost = purchaseCost;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the installation date.
    /// </summary>
    /// <param name="installationDate">The installation date.</param>
    public void SetInstallationDate(DateTime? installationDate)
    {
        InstallationDate = installationDate;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the warranty expiration date.
    /// </summary>
    /// <param name="warrantyExpirationDate">The warranty expiration date.</param>
    public void SetWarrantyExpirationDate(DateTime? warrantyExpirationDate)
    {
        WarrantyExpirationDate = warrantyExpirationDate;
        MarkAsUpdated();
    }

    /// <summary>
    /// Adds a work order to this asset.
    /// </summary>
    /// <param name="workOrder">The work order to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when work order is null.</exception>
    public void AddWorkOrder(WorkOrder workOrder)
    {
        if (workOrder == null) throw new ArgumentNullException(nameof(workOrder));
        if (!_workOrders.Contains(workOrder))
        {
            _workOrders.Add(workOrder);
        }
    }

    /// <summary>
    /// Removes a work order from this asset.
    /// </summary>
    /// <param name="workOrder">The work order to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when work order is null.</exception>
    public void RemoveWorkOrder(WorkOrder workOrder)
    {
        if (workOrder == null) throw new ArgumentNullException(nameof(workOrder));
        _workOrders.Remove(workOrder);
    }

    /// <summary>
    /// Checks if the asset is under warranty.
    /// </summary>
    /// <returns>True if the asset is under warranty, false otherwise.</returns>
    public bool IsUnderWarranty()
    {
        return WarrantyExpirationDate.HasValue && WarrantyExpirationDate.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the asset is operational.
    /// </summary>
    /// <returns>True if the asset is operational, false otherwise.</returns>
    public bool IsOperational()
    {
        return Status == AssetStatus.Operational;
    }
}