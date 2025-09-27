using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for PreventiveMaintenance entities.
/// </summary>
public interface IPreventiveMaintenanceReadOnlyRepository : IReadOnlyRepository<PreventiveMaintenance, Guid>
{
    /// <summary>
    /// Gets preventive maintenance schedules by asset.
    /// </summary>
    /// <param name="assetId">The asset ID.</param>
    /// <returns>List of preventive maintenance schedules for the asset.</returns>
    Task<List<PreventiveMaintenance>> GetByAssetAsync(Guid assetId);

    /// <summary>
    /// Gets preventive maintenance schedules by frequency.
    /// </summary>
    /// <param name="frequency">The maintenance frequency.</param>
    /// <returns>List of preventive maintenance schedules with the specified frequency.</returns>
    Task<List<PreventiveMaintenance>> GetByFrequencyAsync(MaintenanceFrequency frequency);

    /// <summary>
    /// Gets overdue preventive maintenance schedules.
    /// </summary>
    /// <returns>List of overdue preventive maintenance schedules.</returns>
    Task<List<PreventiveMaintenance>> GetOverdueAsync();

    /// <summary>
    /// Gets preventive maintenance schedules due within the specified days.
    /// </summary>
    /// <param name="days">Number of days to check for due maintenance.</param>
    /// <returns>List of preventive maintenance schedules due soon.</returns>
    Task<List<PreventiveMaintenance>> GetDueSoonAsync(int days);

    /// <summary>
    /// Gets paginated preventive maintenance schedules with filtering and sorting.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter by title or description.</param>
    /// <param name="assetId">Filter by asset ID.</param>
    /// <param name="frequency">Filter by frequency.</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction.</param>
    /// <returns>Paginated list of preventive maintenance schedules.</returns>
    Task<PaginatedList<PreventiveMaintenance>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? assetId = null,
        MaintenanceFrequency? frequency = null,
        bool? isActive = null,
        string orderBy = "Title",
        bool orderAscending = true);

    /// <summary>
    /// Searches preventive maintenance schedules by title or description.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>List of matching preventive maintenance schedules.</returns>
    Task<List<PreventiveMaintenance>> SearchAsync(string searchTerm);
}