using EquipTrack.Application.Common.Interfaces;
using EquipTrack.Application.MaintenanceTasks.Queries.GetMaintenanceTaskById;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.MaintenanceTasks.Queries.GetMaintenanceTasks;

/// <summary>
/// Query to get a paginated and filtered list of maintenance tasks.
/// Supports filtering by status, priority, asset, technician, and date range.
/// </summary>
[CacheDuration(2)] // Cache for 2 minutes (dynamic data)
public sealed record GetMaintenanceTasksQuery : IQuery<Result<PagedResult<MaintenanceTaskProjection>>>
{
    // Pagination
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    
    // Filters
    public MaintenanceTaskStatus? Status { get; init; }
    public MaintenanceTaskPriority? Priority { get; init; }
    public MaintenanceTaskType? Type { get; init; }
    public Guid? AssetRef { get; init; }
    public Guid? AssignedTechnicianRef { get; init; }
    public DateTime? ScheduledDateFrom { get; init; }
    public DateTime? ScheduledDateTo { get; init; }
    public bool? IsOverdue { get; init; }
    
    // Sorting
    public string? SortBy { get; init; } // e.g., "ScheduledDate", "Priority", "Status"
    public bool SortDescending { get; init; } = false;
    
    // Search
    public string? SearchTerm { get; init; }
}

/// <summary>
/// Represents a paginated result set.
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}


