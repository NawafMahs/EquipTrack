using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.WorkOrders.Queries;

/// <summary>
/// Query to get all work orders with optional filtering and pagination.
/// </summary>
public sealed record GetWorkOrdersQuery : IRequest<Result<PaginatedList<WorkOrderQuery>>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetWorkOrdersQuery"/>.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="assetRef">Optional asset reference filter.</param>
    /// <param name="assignedToRef">Optional assigned user reference filter.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The page size for pagination.</param>
    /// <param name="searchTerm">Optional search term to filter work orders.</param>
    public GetWorkOrdersQuery(
        WorkOrderStatus? status = null,
        WorkOrderPriority? priority = null,
        Guid? assetRef = null,
        Guid? assignedToRef = null,
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null)
    {
        Status = status;
        Priority = priority;
        AssetRef = assetRef;
        AssignedToRef = assignedToRef;
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }

    /// <summary>
    /// Optional status filter.
    /// </summary>
    public WorkOrderStatus? Status { get; init; }

    /// <summary>
    /// Optional priority filter.
    /// </summary>
    public WorkOrderPriority? Priority { get; init; }

    /// <summary>
    /// Optional asset reference filter.
    /// </summary>
    public Guid? AssetRef { get; init; }

    /// <summary>
    /// Optional assigned user reference filter.
    /// </summary>
    public Guid? AssignedToRef { get; init; }

    /// <summary>
    /// The page number for pagination.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The page size for pagination.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Optional search term to filter work orders.
    /// </summary>
    public string? SearchTerm { get; init; }
}