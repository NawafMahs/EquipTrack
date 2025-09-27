using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for WorkOrderSparePart entities.
/// </summary>
public interface IWorkOrderSparePartWriteOnlyRepository : IWriteOnlyRepository<WorkOrderSparePart, Guid>
{
    /// <summary>
    /// Adds spare parts to a work order in bulk.
    /// </summary>
    /// <param name="workOrderId">The work order ID.</param>
    /// <param name="sparePartUsages">List of spare part usages to add.</param>
    Task AddSparePartsToWorkOrderAsync(Guid workOrderId, IEnumerable<(Guid SparePartId, int QuantityUsed)> sparePartUsages);

    /// <summary>
    /// Removes all spare parts from a work order.
    /// </summary>
    /// <param name="workOrderId">The work order ID.</param>
    Task RemoveAllSparePartsFromWorkOrderAsync(Guid workOrderId);

    /// <summary>
    /// Updates the quantity used for a specific spare part in a work order.
    /// </summary>
    /// <param name="workOrderId">The work order ID.</param>
    /// <param name="sparePartId">The spare part ID.</param>
    /// <param name="newQuantity">The new quantity used.</param>
    Task UpdateQuantityUsedAsync(Guid workOrderId, Guid sparePartId, int newQuantity);
}