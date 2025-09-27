namespace EquipTrack.Query.Projections;

public class SparePartProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsLowStock { get; set; }
    public decimal TotalValue { get; set; }
    public int UsageCount { get; set; }
    public DateTime? LastUsedDate { get; set; }
}

public class SparePartSummaryProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStock { get; set; }
    public bool IsLowStock { get; set; }
    public decimal TotalValue { get; set; }
    public string StockStatus { get; set; } = string.Empty;
}

public class SparePartStatisticsProjection
{
    public int TotalSpareParts { get; set; }
    public int LowStockParts { get; set; }
    public int OutOfStockParts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public decimal AveragePartValue { get; set; }
    
    public List<CategoryCount> PartsByCategory { get; set; } = new();
    public List<SupplierCount> PartsBySupplier { get; set; } = new();
    public List<StockStatusCount> PartsByStockStatus { get; set; } = new();
    public List<TopUsedPart> MostUsedParts { get; set; } = new();
}

public class CategoryCount
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public int LowStockCount { get; set; }
}

public class SupplierCount
{
    public string Supplier { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
}

public class StockStatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class TopUsedPart
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime? LastUsedDate { get; set; }
}