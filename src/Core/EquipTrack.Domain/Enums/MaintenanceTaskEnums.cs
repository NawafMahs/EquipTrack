namespace EquipTrack.Domain.Enums;

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

/// <summary>
/// Represents the frequency of preventive maintenance.
/// </summary>
public enum MaintenanceFrequency
{
    Daily = 1,
    Weekly = 2,
    Biweekly = 3,
    Monthly = 4,
    Quarterly = 5,
    SemiAnnually = 6,
    Annually = 7,
    Custom = 99
}

/// <summary>
/// Represents the category of spare parts in the CMMS system.
/// </summary>
public enum SparePartCategory
{
    Mechanical = 1,
    Electrical = 2,
    Electronic = 3,
    Hydraulic = 4,
    Pneumatic = 5,
    Consumable = 6,
    SafetyEquipment = 7,
    Tools = 8,
    Other = 99
}

/// <summary>
/// Represents the status of spare part stock levels.
/// </summary>
public enum StockStatus
{
    InStock = 1,
    LowStock = 2,
    OutOfStock = 3,
    OnOrder = 4,
    Discontinued = 5
}


