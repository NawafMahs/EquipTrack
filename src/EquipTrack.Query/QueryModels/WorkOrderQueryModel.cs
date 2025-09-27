using EquipTrack.Domain.Enums;

namespace EquipTrack.Query.QueryModels;

public class WorkOrderQueryModel
{
    public WorkOrderStatus? Status { get; set; }
    public WorkOrderPriority? Priority { get; set; }
    public WorkOrderType? Type { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? RequestedDateFrom { get; set; }
    public DateTime? RequestedDateTo { get; set; }
    public DateTime? ScheduledDateFrom { get; set; }
    public DateTime? ScheduledDateTo { get; set; }
    public DateTime? CompletedDateFrom { get; set; }
    public DateTime? CompletedDateTo { get; set; }
    public bool? IsOverdue { get; set; }
    public decimal? MinEstimatedCost { get; set; }
    public decimal? MaxEstimatedCost { get; set; }
    public string? AssetLocation { get; set; }
    public string? SortBy { get; set; } = "RequestedDate";
    public string? SortDirection { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class WorkOrderDashboardQueryModel
{
    public Guid? UserId { get; set; }
    public bool? MyWorkOrders { get; set; }
    public int DaysRange { get; set; } = 30;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}