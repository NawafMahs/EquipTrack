using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for SparePart entities.
/// </summary>
public interface ISparePartWriteOnlyRepository : IWriteOnlyRepository<SparePart, Guid>
{
    /// <summary>
    /// Updates the stock quantity for a spare part.
    /// </summary>
    /// <param name="sparePartId">The spare part ID.</param>
    /// <param name="newQuantity">The new quantity.</param>
    /// <param name="updatedBy">User who updated the quantity.</param>
    Task UpdateStockQuantityAsync(Guid sparePartId, int newQuantity, string updatedBy);

    /// <summary>
    /// Decreases the stock quantity for a spare part (used when parts are consumed).
    /// </summary>
    /// <param name="sparePartId">The spare part ID.</param>
    /// <param name="quantityUsed">The quantity to decrease.</param>
    /// <param name="updatedBy">User who updated the quantity.</param>
    Task DecreaseStockAsync(Guid sparePartId, int quantityUsed, string updatedBy);

    /// <summary>
    /// Increases the stock quantity for a spare part (used when parts are restocked).
    /// </summary>
    /// <param name="sparePartId">The spare part ID.</param>
    /// <param name="quantityAdded">The quantity to increase.</param>
    /// <param name="updatedBy">User who updated the quantity.</param>
    Task IncreaseStockAsync(Guid sparePartId, int quantityAdded, string updatedBy);
}