using EquipTrack.Domain.Enums;

namespace EquipTrack.Query.Projections;

public class WorkOrderProjection
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
    public WorkOrderStatus Status { get; set; }
    public WorkOrderType Type { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public string? CompletionNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Asset information
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string AssetSerialNumber { get; set; } = string.Empty;
    public string AssetLocation { get; set; } = string.Empty;

    // User information
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public string CreatedByUserEmail { get; set; } = string.Empty;
    
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string? AssignedToUserEmail { get; set; }

    // Spare parts summary
    public int SparePartsCount { get; set; }
    public decimal SparePartsTotalCost { get; set; }
}

public class WorkOrderSummaryProjection
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
    public WorkOrderStatus Status { get; set; }
    public WorkOrderType Type { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string AssetLocation { get; set; } = string.Empty;
    public string? AssignedToUserName { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public bool IsOverdue { get; set; }
    public int DaysOpen { get; set; }
}

public class WorkOrderStatisticsProjection
{
    public int TotalWorkOrders { get; set; }
    public int OpenWorkOrders { get; set; }
    public int InProgressWorkOrders { get; set; }
    public int CompletedWorkOrders { get; set; }
    public int CancelledWorkOrders { get; set; }
    public int OverdueWorkOrders { get; set; }
    
    public decimal TotalEstimatedCost { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal AverageCompletionTime { get; set; }
    
    public List<WorkOrderStatusCount> WorkOrdersByStatus { get; set; } = new();
    public List<WorkOrderPriorityCount> WorkOrdersByPriority { get; set; } = new();
    public List<WorkOrderTypeCount> WorkOrdersByType { get; set; } = new();
    public List<MonthlyWorkOrderCount> MonthlyTrends { get; set; } = new();
}

public class WorkOrderStatusCount
{
    public WorkOrderStatus Status { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class WorkOrderPriorityCount
{
    public WorkOrderPriority Priority { get; set; }
    public int Count { get; set; }
    public decimal AverageCompletionDays { get; set; }
}

public class WorkOrderTypeCount
{
    public WorkOrderType Type { get; set; }
    public int Count { get; set; }
    public decimal TotalCost { get; set; }
}

public class MonthlyWorkOrderCount
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalCost { get; set; }
}