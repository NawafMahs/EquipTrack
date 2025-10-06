namespace EquipTrack.Domain.Aggregates.Maintenance.Enums;

/// <summary>
/// Represents the priority level of a maintenance task.
/// </summary>
public enum MaintenanceTaskPriority
{
    /// <summary>
    /// Low priority - can be scheduled flexibly
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Medium priority - should be completed in reasonable time
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// High priority - should be completed soon
    /// </summary>
    High = 3,
    
    /// <summary>
    /// Critical priority - requires immediate attention
    /// </summary>
    Critical = 4,
    
    /// <summary>
    /// Emergency - production stoppage or safety issue
    /// </summary>
    Emergency = 5
}