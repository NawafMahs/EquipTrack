using EquipTrack.Application.Common.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.MaintenanceTasks.Commands.CreateMaintenanceTask;

/// <summary>
/// Command to create a new maintenance task in the CMMS system.
/// Follows CQRS pattern - no DTOs, only Commands and Queries.
/// </summary>
public sealed record CreateMaintenanceTaskCommand : ICommand<Result<Guid>>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public MaintenanceTaskType Type { get; init; }
    public MaintenanceTaskPriority Priority { get; init; }
    public Guid AssetRef { get; init; }
    public int CreatedByRef { get; init; }
    public int? AssignedTechnicianRef { get; init; }
    public DateTime ScheduledDate { get; init; }
    public decimal EstimatedHours { get; init; }
    public decimal EstimatedCost { get; init; }
}


