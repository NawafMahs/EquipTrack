using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for WorkOrder entities.
/// </summary>
public interface IWorkOrderReadOnlyRepository : IReadOnlyRepository<WorkOrder, Guid>
{
    /// <summary>
    /// Gets work orders by status.
    /// </summary>
    /// <param name="status">The status to filter by.</param>
    /// <returns>List of work orders with the specified status.</returns>
    Task<List<WorkOrder>> GetByStatusAsync(WorkOrderStatus status);

    /// <summary>
    /// Gets work orders by priority.
    /// </summary>
    /// <param name="priority">The priority to filter by.</param>
    /// <returns>List of work orders with the specified priority.</returns>
    Task<List<WorkOrder>> GetByPriorityAsync(WorkOrderPriority priority);

    /// <summary>
    /// Gets work orders assigned to a specific user.
    /// </summary>
    /// <param name="userRef">The user reference ID.</param>
    /// <returns>List of work orders assigned to the user.</returns>
    Task<List<WorkOrder>> GetByAssignedUserAsync(Guid userRef);

    /// <summary>
    /// Gets work orders for a specific asset.
    /// </summary>
    /// <param name="assetRef">The asset reference ID.</param>
    /// <returns>List of work orders for the asset.</returns>
    Task<List<WorkOrder>> GetByAssetAsync(Guid assetRef);

    /// <summary>
    /// Checks if there are any active work orders for the specified asset.
    /// </summary>
    /// <param name="assetRef">The asset reference ID.</param>
    /// <returns>True if there are active work orders; otherwise, false.</returns>
    Task<bool> HasActiveWorkOrdersForAssetAsync(Guid assetRef);

    /// <summary>
    /// Gets overdue work orders.
    /// </summary>
    /// <returns>List of overdue work orders.</returns>
    Task<List<WorkOrder>> GetOverdueAsync();

    /// <summary>
    /// Searches work orders by title or description.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>List of matching work orders.</returns>
    Task<List<WorkOrder>> SearchAsync(string searchTerm);
}