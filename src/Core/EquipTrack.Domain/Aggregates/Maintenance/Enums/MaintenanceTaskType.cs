namespace EquipTrack.Domain.Aggregates.Maintenance.Enums;

/// <summary>
/// Represents the type of maintenance task.
/// </summary>
public enum MaintenanceTaskType
{
    /// <summary>
    /// Corrective maintenance - fixing a problem
    /// </summary>
    Corrective = 1,
    
    /// <summary>
    /// Preventive maintenance - scheduled routine maintenance
    /// </summary>
    Preventive = 2,
    
    /// <summary>
    /// Predictive maintenance - based on condition monitoring
    /// </summary>
    Predictive = 3,
    
    /// <summary>
    /// Emergency maintenance - critical issues requiring immediate attention
    /// </summary>
    Emergency = 4,
    
    /// <summary>
    /// Inspection - routine inspection of equipment
    /// </summary>
    Inspection = 5,
    
    /// <summary>
    /// Calibration - equipment calibration
    /// </summary>
    Calibration = 6
}