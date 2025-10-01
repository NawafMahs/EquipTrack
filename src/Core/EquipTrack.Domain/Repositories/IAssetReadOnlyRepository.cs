using EquipTrack.Domain.Common;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for Asset entities.
/// </summary>
public interface IAssetReadOnlyRepository : IReadOnlyRepository<Asset, Guid>
{
    /// <summary>
    /// Checks if an asset exists with the specified asset tag.
    /// </summary>
    /// <param name="assetTag">The asset tag to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <returns>True if an asset exists with the specified asset tag; otherwise, false.</returns>
    Task<bool> ExistsByAssetTagAsync(string assetTag, Guid? excludeId = null);

    /// <summary>
    /// Checks if an asset exists with the specified serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <returns>True if an asset exists with the specified serial number; otherwise, false.</returns>
    Task<bool> ExistsBySerialNumberAsync(string serialNumber, Guid? excludeId = null);

    /// <summary>
    /// Gets assets by location with pagination.
    /// </summary>
    /// <param name="location">The location to filter by.</param>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction.</param>
    /// <returns>Paginated list of assets in the specified location.</returns>
    Task<PaginatedList<Asset>> GetByLocationPagedAsync(
        string location,
        int pageNumber,
        int pageSize,
        string orderBy = "Name",
        bool orderAscending = true);

    /// <summary>
    /// Gets assets by status.
    /// </summary>
    /// <param name="status">The status to filter by.</param>
    /// <returns>List of assets with the specified status.</returns>
    Task<List<Asset>> GetByStatusAsync(AssetStatus status);

    /// <summary>
    /// Gets assets by criticality.
    /// </summary>
    /// <param name="criticality">The criticality to filter by.</param>
    /// <returns>List of assets with the specified criticality.</returns>
    Task<List<Asset>> GetByCriticalityAsync(AssetCriticality criticality);

    /// <summary>
    /// Gets paginated assets with filtering and sorting.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter by name, asset tag, or serial number.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="criticality">Filter by criticality.</param>
    /// <param name="location">Filter by location.</param>
    /// <param name="manufacturer">Filter by manufacturer.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction.</param>
    /// <returns>Paginated list of assets.</returns>
    Task<PaginatedList<Asset>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        AssetStatus? status = null,
        AssetCriticality? criticality = null,
        string? location = null,
        string? manufacturer = null,
        string orderBy = "Name",
        bool orderAscending = true);

    /// <summary>
    /// Searches assets by name, asset tag, serial number, model, or manufacturer.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>List of matching assets.</returns>
    Task<List<Asset>> SearchAsync(string searchTerm);

    /// <summary>
    /// Gets assets that are under warranty.
    /// </summary>
    /// <returns>List of assets under warranty.</returns>
    Task<List<Asset>> GetAssetsUnderWarrantyAsync();

    /// <summary>
    /// Gets assets with warranty expiring within the specified days.
    /// </summary>
    /// <param name="days">Number of days to check for warranty expiration.</param>
    /// <returns>List of assets with warranty expiring soon.</returns>
    Task<List<Asset>> GetAssetsWithWarrantyExpiringAsync(int days);
}