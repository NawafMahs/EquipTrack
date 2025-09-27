using EquipTrack.Domain.Enums;

namespace EquipTrack.Query.Projections;

public class AssetProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

public class AssetSummaryProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public decimal PurchasePrice { get; set; }
    public int ActiveWorkOrdersCount { get; set; }
    public int TotalWorkOrdersCount { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

public class AssetStatisticsProjection
{
    public int TotalAssets { get; set; }
    public int ActiveAssets { get; set; }
    public int InactiveAssets { get; set; }
    public int UnderMaintenanceAssets { get; set; }
    public int DisposedAssets { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageValue { get; set; }
    public List<AssetStatusCount> AssetsByStatus { get; set; } = new();
    public List<ManufacturerCount> TopManufacturers { get; set; } = new();
    public List<LocationCount> AssetsByLocation { get; set; } = new();
}

public class AssetStatusCount
{
    public AssetStatus Status { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class ManufacturerCount
{
    public string Manufacturer { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
}

public class LocationCount
{
    public string Location { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
}