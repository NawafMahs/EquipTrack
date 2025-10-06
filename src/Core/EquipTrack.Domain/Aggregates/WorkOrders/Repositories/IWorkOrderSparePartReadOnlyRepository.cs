using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for WorkOrderSparePart entities.
/// </summary>
public interface IWorkOrderSparePartReadOnlyRepository : IReadOnlyRepository<WorkOrderSparePart, Guid>
{
    /// <summary>
    /// Gets spare parts used in a specific work order.
    /// </summary>
    /// <param name="workOrderId">The work order ID.</param>
    /// <returns>List of spare parts used in the work order.</returns>
    Task<List<WorkOrderSparePart>> GetByWorkOrderAsync(Guid workOrderId);

    /// <summary>
    /// Gets work orders that used a specific spare part.
    /// </summary>
    /// <param name="sparePartId">The spare part ID.</param>
    /// <returns>List of work order spare part relationships for the spare part.</returns>
    Task<List<WorkOrderSparePart>> GetBySparePartAsync(Guid sparePartId);

    /// <summary>
    /// Gets the total quantity of a spare part used across all work orders.
    /// </summary>
    /// <param name="sparePartId">The spare part ID.</param>
    /// <returns>Total quantity used.</returns>
    Task<int> GetTotalQuantityUsedAsync(Guid sparePartId);

    /// <summary>
    /// Gets spare part usage statistics for a date range.
    /// </summary>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    /// <returns>List of spare part usage statistics.</returns>
    Task<List<WorkOrderSparePart>> GetUsageStatisticsAsync(DateTime startDate, DateTime endDate);
}