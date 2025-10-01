using EquipTrack.Domain.Enums;
using EquipTrack.Query.Abstractions;

namespace EquipTrack.Query.QueryModels;

public record AssetQueryModel : IQueryModel<Guid>
{
    public AssetQueryModel(Guid id,
                           AssetStatus? status,
                           string? location,
                           string? manufacturer,
                           string? category,
                           string? searchTerm,
                           DateTime? purchaseDateFrom,
                           DateTime? purchaseDateTo,
                           decimal? minValue,
                           decimal? maxValue,
                           bool? hasActiveWorkOrders,
                           bool? isUnderWarranty,
                           string? sortBy,
                           string? sortDirection,
                           int page,
                           int pageSize)
    {
        Id = id;
        Status = status;
        Location = location;
        Manufacturer = manufacturer;
        Category = category;
        SearchTerm = searchTerm;
        PurchaseDateFrom = purchaseDateFrom;
        PurchaseDateTo = purchaseDateTo;
        MinValue = minValue;
        MaxValue = maxValue;
        HasActiveWorkOrders = hasActiveWorkOrders;
        IsUnderWarranty = isUnderWarranty;
        SortBy = sortBy;
        SortDirection = sortDirection;
        Page = page;
        PageSize = pageSize;
    }

    public Guid Id { get; set; }
    public AssetStatus? Status { get; set; }
    public string? Location { get; set; }
    public string? Manufacturer { get; set; }
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? PurchaseDateFrom { get; set; }
    public DateTime? PurchaseDateTo { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public bool? HasActiveWorkOrders { get; set; }
    public bool? IsUnderWarranty { get; set; }
    public string? SortBy { get; set; } = "Name";
    public string? SortDirection { get; set; } = "ASC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    
    
}

public class AssetMaintenanceQueryModel
{
    public Guid? AssetId { get; set; }
    public string? Location { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsDueSoon { get; set; }
    public int DueSoonDays { get; set; } = 7;
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? SortBy { get; set; } = "NextDueDate";
    public string? SortDirection { get; set; } = "ASC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}