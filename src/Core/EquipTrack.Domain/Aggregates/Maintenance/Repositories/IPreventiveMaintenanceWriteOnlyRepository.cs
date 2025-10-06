using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for PreventiveMaintenance entities.
/// </summary>
public interface IPreventiveMaintenanceWriteOnlyRepository : IWriteOnlyRepository<PreventiveMaintenance, Guid>
{
    /// <summary>
    /// Updates the next due date for a preventive maintenance schedule.
    /// </summary>
    /// <param name="preventiveMaintenanceId">The preventive maintenance ID.</param>
    /// <param name="nextDueDate">The new next due date.</param>
    /// <param name="updatedBy">User who updated the schedule.</param>
    Task UpdateNextDueDateAsync(Guid preventiveMaintenanceId, DateTime nextDueDate, string updatedBy);

    /// <summary>
    /// Marks a preventive maintenance schedule as completed and updates the next due date.
    /// </summary>
    /// <param name="preventiveMaintenanceId">The preventive maintenance ID.</param>
    /// <param name="completedDate">The completion date.</param>
    /// <param name="nextDueDate">The next due date.</param>
    /// <param name="updatedBy">User who completed the maintenance.</param>
    Task MarkAsCompletedAsync(Guid preventiveMaintenanceId, DateTime completedDate, DateTime nextDueDate, string updatedBy);

    /// <summary>
    /// Activates or deactivates a preventive maintenance schedule.
    /// </summary>
    /// <param name="preventiveMaintenanceId">The preventive maintenance ID.</param>
    /// <param name="isActive">The active status.</param>
    /// <param name="updatedBy">User who updated the status.</param>
    Task SetActiveStatusAsync(Guid preventiveMaintenanceId, bool isActive, string updatedBy);
}