namespace EquipTrack.Domain.Enums;

/// <summary>
/// Types of log entries for assets.
/// Unified log type for all asset types.
/// </summary>
public enum AssetLogType
{
    /// <summary>
    /// Asset status changed
    /// </summary>
    StatusChange = 0,

    /// <summary>
    /// Maintenance activity performed
    /// </summary>
    MaintenancePerformed = 1,

    /// <summary>
    /// Error occurred
    /// </summary>
    ErrorOccurred = 2,

    /// <summary>
    /// Warning issued
    /// </summary>
    WarningIssued = 3,

    /// <summary>
    /// Configuration changed
    /// </summary>
    ConfigurationChanged = 4,

    /// <summary>
    /// Sensor alert triggered
    /// </summary>
    SensorAlert = 5,

    /// <summary>
    /// Battery alert (for robots/electric vehicles)
    /// </summary>
    BatteryAlert = 6,

    /// <summary>
    /// Performance alert
    /// </summary>
    PerformanceAlert = 7,

    /// <summary>
    /// Message received from RabbitMQ
    /// </summary>
    RabbitMQMessage = 8,

    /// <summary>
    /// Message received from MQTT
    /// </summary>
    MQTTMessage = 9,

    /// <summary>
    /// Manual entry by user
    /// </summary>
    ManualEntry = 10,

    /// <summary>
    /// Task assigned to robot
    /// </summary>
    TaskAssigned = 11,

    /// <summary>
    /// Task completed by robot
    /// </summary>
    TaskCompleted = 12,

    /// <summary>
    /// Firmware update performed
    /// </summary>
    FirmwareUpdate = 13,

    /// <summary>
    /// Operation started (for machines)
    /// </summary>
    OperationStart = 14,

    /// <summary>
    /// Operation ended (for machines)
    /// </summary>
    OperationEnd = 15,

    /// <summary>
    /// Spare part replacement performed
    /// </summary>
    SparePartReplacement = 16,

    /// <summary>
    /// Calibration performed
    /// </summary>
    Calibration = 17,

    /// <summary>
    /// Collision detected (for robots)
    /// </summary>
    CollisionDetected = 18
}