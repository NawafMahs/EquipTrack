using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Assets.Enums;

using EquipTrack.Application.DTOs;
namespace EquipTrack.Application.Assets.Queries;

/// <summary>
/// Query to get a paginated list of assets.
/// </summary>
public sealed record GetAssetsQuery : IRequest<Result<PaginatedList<AssetQuery>>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetAssetsQuery"/>.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter assets by name, asset tag, or serial number.</param>
    /// <param name="status">Filter by asset status.</param>
    /// <param name="criticality">Filter by asset criticality.</param>
    /// <param name="location">Filter by asset location.</param>
    /// <param name="manufacturer">Filter by manufacturer.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    public GetAssetsQuery(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        AssetStatus? status = null,
        AssetCriticality? criticality = null,
        string? location = null,
        string? manufacturer = null,
        string orderBy = "Name",
        bool orderAscending = true)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        Status = status;
        Criticality = criticality;
        Location = location;
        Manufacturer = manufacturer;
        OrderBy = orderBy;
        OrderAscending = orderAscending;
    }

    /// <summary>
    /// Page number (starts from 1).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Search term to filter assets by name, asset tag, or serial number.
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by asset status.
    /// </summary>
    public AssetStatus? Status { get; init; }

    /// <summary>
    /// Filter by asset criticality.
    /// </summary>
    public AssetCriticality? Criticality { get; init; }

    /// <summary>
    /// Filter by asset location.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Filter by manufacturer.
    /// </summary>
    public string? Manufacturer { get; init; }

    /// <summary>
    /// Field to order by (Name, AssetTag, SerialNumber, CreatedAt, etc.).
    /// </summary>
    public string OrderBy { get; init; }

    /// <summary>
    /// Order direction (true for ascending, false for descending).
    /// </summary>
    public bool OrderAscending { get; init; }
}