using MediatR;
using EquipTrack.Core.SharedKernel;
namespace EquipTrack.Application.Assets.Queries;

/// <summary>
/// Query to get assets by location.
/// </summary>
public sealed record GetAssetsByLocationQuery : IRequest<Result<PaginatedList<AssetQuery>>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetAssetsByLocationQuery"/>.
    /// </summary>
    /// <param name="location">Location to filter by.</param>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    public GetAssetsByLocationQuery(
        string location,
        int pageNumber = 1,
        int pageSize = 20,
        string orderBy = "Name",
        bool orderAscending = true)
    {
        Location = location;
        PageNumber = pageNumber;
        PageSize = pageSize;
        OrderBy = orderBy;
        OrderAscending = orderAscending;
    }

    /// <summary>
    /// Location to filter by.
    /// </summary>
    public string Location { get; init; }

    /// <summary>
    /// Page number (starts from 1).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Field to order by.
    /// </summary>
    public string OrderBy { get; init; }

    /// <summary>
    /// Order direction (true for ascending, false for descending).
    /// </summary>
    public bool OrderAscending { get; init; }
}