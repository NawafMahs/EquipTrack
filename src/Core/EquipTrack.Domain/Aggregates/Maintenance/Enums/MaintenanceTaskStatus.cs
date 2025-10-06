namespace EquipTrack.Domain.Aggregates.Maintenance.Enums;

/// <summary>
/// Represents the status of a maintenance task.
/// </summary>
public enum MaintenanceTaskStatus
{
    /// <summary>
    /// Task is scheduled but not yet assigned
    /// </summary>
    Scheduled = 1,
    
    /// <summary>
    /// Task is assigned to a technician
    /// </summary>
    Assigned = 2,
    
    /// <summary>
    /// Task is currently in progress
    /// </summary>
    InProgress = 3,
    
    /// <summary>
    /// Task is on hold
    /// </summary>
    OnHold = 4,
    
    /// <summary>
    /// Task is completed
    /// </summary>
    Completed = 5,
    
    /// <summary>
    /// Task is cancelled
    /// </summary>
    Cancelled = 6,
    
    /// <summary>
    /// Task requires review or approval
    /// </summary>
    PendingReview = 7
}