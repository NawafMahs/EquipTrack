using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for MaintenanceTask entity.
/// Provides query operations following CQRS pattern.
/// </summary>
public interface IMaintenanceTaskReadOnlyRepository : IReadOnlyRepository<MaintenanceTask, Guid>
{
    /// <summary>
    /// Checks if a maintenance task exists by ID.
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets maintenance tasks by asset reference.
    /// </summary>
    Task<IEnumerable<MaintenanceTask>> GetByAssetRefAsync(
        Guid assetRef, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets maintenance tasks assigned to a specific technician.
    /// </summary>
    Task<IEnumerable<MaintenanceTask>> GetByTechnicianRefAsync(
        Guid technicianRef, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets overdue maintenance tasks.
    /// </summary>
    Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets maintenance tasks scheduled within a date range.
    /// </summary>
    Task<IEnumerable<MaintenanceTask>> GetTasksByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}


