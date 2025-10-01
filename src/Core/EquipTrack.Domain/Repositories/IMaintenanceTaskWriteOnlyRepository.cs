using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for MaintenanceTask entity.
/// Provides write operations following CQRS pattern.
/// </summary>
public interface IMaintenanceTaskWriteOnlyRepository : IWriteOnlyRepository<MaintenanceTask, Guid>
{
    /// <summary>
    /// Adds a new maintenance task asynchronously.
    /// </summary>
    Task AddAsync(MaintenanceTask maintenanceTask, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds multiple maintenance tasks in a batch operation.
    /// </summary>
    Task AddRangeAsync(
        IEnumerable<MaintenanceTask> maintenanceTasks, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a maintenance task by ID asynchronously.
    /// </summary>
    Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default);
}


