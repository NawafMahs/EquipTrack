namespace EquipTrack.Query.QueryModels;

public class SparePartQueryModel
{
    public string? Category { get; set; }
    public string? Supplier { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsLowStock { get; set; }
    public bool? IsOutOfStock { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public string? Location { get; set; }
    public string? SortBy { get; set; } = "Name";
    public string? SortDirection { get; set; } = "ASC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SparePartUsageQueryModel
{
    public Guid? SparePartId { get; set; }
    public DateTime? UsageDateFrom { get; set; }
    public DateTime? UsageDateTo { get; set; }
    public Guid? WorkOrderId { get; set; }
    public string? Category { get; set; }
    public string? SortBy { get; set; } = "UsageDate";
    public string? SortDirection { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}