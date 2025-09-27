using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for SparePart entities.
/// </summary>
public interface ISparePartReadOnlyRepository : IReadOnlyRepository<SparePart, Guid>
{
    /// <summary>
    /// Checks if a spare part exists with the specified part number.
    /// </summary>
    /// <param name="partNumber">The part number to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <returns>True if a spare part exists with the specified part number; otherwise, false.</returns>
    Task<bool> ExistsByPartNumberAsync(string partNumber, Guid? excludeId = null);

    /// <summary>
    /// Gets spare parts with low stock (quantity below minimum stock level).
    /// </summary>
    /// <returns>List of spare parts with low stock.</returns>
    Task<List<SparePart>> GetLowStockPartsAsync();

    /// <summary>
    /// Gets spare parts by supplier.
    /// </summary>
    /// <param name="supplier">The supplier name.</param>
    /// <returns>List of spare parts from the specified supplier.</returns>
    Task<List<SparePart>> GetBySupplierAsync(string supplier);

    /// <summary>
    /// Gets paginated spare parts with filtering and sorting.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter by name or part number.</param>
    /// <param name="supplier">Filter by supplier.</param>
    /// <param name="category">Filter by category.</param>
    /// <param name="lowStock">Filter by low stock status.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction.</param>
    /// <returns>Paginated list of spare parts.</returns>
    Task<PaginatedList<SparePart>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? supplier = null,
        string? category = null,
        bool? lowStock = null,
        string orderBy = "Name",
        bool orderAscending = true);

    /// <summary>
    /// Searches spare parts by name, part number, or description.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>List of matching spare parts.</returns>
    Task<List<SparePart>> SearchAsync(string searchTerm);
}