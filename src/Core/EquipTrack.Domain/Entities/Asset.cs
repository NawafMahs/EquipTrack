using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;


public class Asset : BaseEntity, IAggregateRoot
{
    // Simple properties with init
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public string Model { get; init; } = default!;
    public string Manufacturer { get; init; } = default!;
    public DateTime PurchaseDate { get; init; }
    public decimal PurchasePrice { get; init; } 

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
        decimal purchasePrice)
    {
        Name = Ensure.NotEmpty(name, nameof(name));
        Description = Ensure.NotEmpty(description, nameof(description));
        SerialNumber = Ensure.NotEmpty(serialNumber, nameof(serialNumber));
        Model = Ensure.NotEmpty(model, nameof(model));
        Manufacturer = Ensure.NotEmpty(manufacturer, nameof(manufacturer));
        Location = Ensure.NotEmpty(location, nameof(location));
        PurchaseDate = purchaseDate;
        PurchasePrice = Ensure.Positive(purchasePrice, nameof(purchasePrice));
    }
    protected Asset(DateTime purchaseDate, decimal purchasePrice)
    {
        PurchaseDate = purchaseDate;
        PurchasePrice = purchasePrice;
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

    public Asset SetPurchaseInfo(DateTime purchaseDate, decimal purchasePrice) => new(purchaseDate, purchasePrice);
    
    public bool IsUnderWarranty() => WarrantyExpiryDate.HasValue && WarrantyExpiryDate.Value > DateTime.UtcNow;


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
