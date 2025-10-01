using EquipTrack.Application.Common.Interfaces;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.MaintenanceTasks.Queries.GetMaintenanceTaskById;

/// <summary>
/// Query to get a maintenance task by its ID.
/// Returns a projection of the maintenance task with related data.
/// </summary>
public sealed record GetMaintenanceTaskByIdQuery(Guid Id) : IQuery<Result<MaintenanceTaskProjection>>;

/// <summary>
/// Projection for MaintenanceTask entity.
/// Used instead of DTOs - provides read-optimized view of the data.
/// </summary>
public sealed record MaintenanceTaskProjection
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    
    // Foreign key references
    public Guid AssetRef { get; init; }
    public string AssetName { get; init; } = string.Empty;
    public string AssetSerialNumber { get; init; } = string.Empty;
    
    public Guid? AssignedTechnicianRef { get; init; }
    public string? AssignedTechnicianName { get; init; }
    
    public Guid CreatedByRef { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    
    // Date information
    public DateTime ScheduledDate { get; init; }
    public DateTime? StartedDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    
    // Cost and time tracking
    public decimal EstimatedHours { get; init; }
    public decimal ActualHours { get; init; }
    public decimal EstimatedCost { get; init; }
    public decimal ActualCost { get; init; }
    public decimal HoursVariance { get; init; }
    public decimal CostVariance { get; init; }
    
    // Additional information
    public string? CompletionNotes { get; init; }
    public bool IsOverdue { get; init; }
    
    // Spare parts used
    public IReadOnlyList<SparePartUsageProjection> SpareParts { get; init; } 
        = Array.Empty<SparePartUsageProjection>();
    
    // Audit fields
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Projection for spare part usage in maintenance tasks.
/// </summary>
public sealed record SparePartUsageProjection
{
    public Guid SparePartRef { get; init; }
    public string SparePartName { get; init; } = string.Empty;
    public string PartNumber { get; init; } = string.Empty;
    public int QuantityUsed { get; init; }
    public decimal UnitCost { get; init; }
    public decimal TotalCost { get; init; }
}


