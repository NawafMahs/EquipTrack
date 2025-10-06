using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents a robot asset in the CMMS system.
/// Extends Asset with robot-specific properties and behaviors.
/// </summary>
public class Robot : Asset
{
    #region Robot-Specific Properties

    /// <summary>
    /// Type/category of the robot (Industrial, Collaborative, Mobile, etc.)
    /// </summary>
    public RobotType RobotType { get; private set; }

    /// <summary>
    /// Maximum payload capacity in kilograms
    /// </summary>
    public int MaxPayloadKg { get; private set; }

    /// <summary>
    /// Maximum reach distance in meters
    /// </summary>
    public decimal ReachMeters { get; private set; }

    /// <summary>
    /// Current battery level percentage (0-100)
    /// Null for non-battery powered robots
    /// </summary>
    public decimal? BatteryLevel { get; private set; }

    /// <summary>
    /// Description of the current task being performed
    /// </summary>
    public string? CurrentTask { get; private set; }

    /// <summary>
    /// Total number of operational cycles completed
    /// </summary>
    public int CycleCount { get; private set; }

    /// <summary>
    /// Current firmware version installed
    /// </summary>
    public string? FirmwareVersion { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// EF Core parameterless constructor
    /// </summary>
    private Robot() : base() { }

    /// <summary>
    /// Private constructor for factory method
    /// </summary>
    private Robot(
        string name,
        string description,
        string serialNumber,
        string model,
        string manufacturer,
        string assetTag,
        string location,
        DateTime purchaseDate,
        decimal purchasePrice,
        AssetCriticality criticality,
        RobotType robotType,
        int maxPayloadKg,
        decimal reachMeters,
        string? firmwareVersion = null)
        : base(
            name,
            description,
            serialNumber,
            model,
            manufacturer,
            assetTag,
            AssetType.Robot,
            location,
            purchaseDate,
            purchasePrice,
            criticality)
    {
        RobotType = robotType;
        MaxPayloadKg = Ensure.Positive(maxPayloadKg, nameof(maxPayloadKg));
        ReachMeters = Ensure.Positive(reachMeters, nameof(reachMeters));
        FirmwareVersion = firmwareVersion;
        BatteryLevel = null; // Initialize as null, set later if applicable
        CycleCount = 0;
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates a new Robot instance with validation
    /// </summary>
    public static Robot Create(
        string name,
        string description,
        string serialNumber,
        string model,
        string manufacturer,
        string assetTag,
        string location,
        DateTime purchaseDate,
        decimal purchasePrice,
        RobotType robotType,
        int maxPayloadKg,
        decimal reachMeters,
        AssetCriticality criticality = AssetCriticality.Medium,
        string? firmwareVersion = null)
    {
        var robot = new Robot(
            name,
            description,
            serialNumber,
            model,
            manufacturer,
            assetTag,
            location,
            purchaseDate,
            purchasePrice,
            criticality,
            robotType,
            maxPayloadKg,
            reachMeters,
            firmwareVersion);

        robot.AddLog(
            AssetLogType.StatusChange,
            $"Robot created: {name} ({robotType})",
            LogSeverity.Info);

        return robot;
    }

    #endregion

    #region Robot-Specific Behaviors

    /// <summary>
    /// Updates the robot's battery level
    /// </summary>
    public void UpdateBatteryLevel(decimal batteryLevel)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentException("Battery level must be between 0 and 100.", nameof(batteryLevel));

        var oldLevel = BatteryLevel;
        BatteryLevel = batteryLevel;

        // Log battery alerts
        if (batteryLevel <= 10)
        {
            AddLog(AssetLogType.BatteryAlert, $"Critical battery level: {batteryLevel}%", LogSeverity.Critical);
        }
        else if (batteryLevel <= 20)
        {
            AddLog(AssetLogType.BatteryAlert, $"Low battery level: {batteryLevel}%", LogSeverity.Warning);
        }
        else if (oldLevel.HasValue && oldLevel <= 20 && batteryLevel > 20)
        {
            AddLog(AssetLogType.BatteryAlert, $"Battery level restored: {batteryLevel}%", LogSeverity.Info);
        }
    }

    /// <summary>
    /// Assigns a task to the robot
    /// </summary>
    public void AssignTask(string task)
    {
        CurrentTask = Ensure.NotEmpty(task, nameof(task));
        AddLog(AssetLogType.TaskAssigned, $"Task assigned: {task}", LogSeverity.Info);
    }

    /// <summary>
    /// Completes the current task and increments cycle count
    /// </summary>
    public void CompleteTask()
    {
        if (string.IsNullOrWhiteSpace(CurrentTask))
            throw new InvalidOperationException("No task is currently assigned.");

        var completedTask = CurrentTask;
        CurrentTask = null;
        CycleCount++;

        AddLog(AssetLogType.TaskCompleted, $"Task completed: {completedTask}. Total cycles: {CycleCount}", LogSeverity.Info);
    }

    /// <summary>
    /// Clears the current task without completing it
    /// </summary>
    public void ClearTask()
    {
        if (!string.IsNullOrWhiteSpace(CurrentTask))
        {
            var clearedTask = CurrentTask;
            CurrentTask = null;
            AddLog(AssetLogType.ManualEntry, $"Task cleared: {clearedTask}", LogSeverity.Info);
        }
    }

    /// <summary>
    /// Updates the robot's firmware version
    /// </summary>
    public void UpdateFirmware(string newVersion)
    {
        var oldVersion = FirmwareVersion ?? "Unknown";
        FirmwareVersion = Ensure.NotEmpty(newVersion, nameof(newVersion));
        AddLog(AssetLogType.FirmwareUpdate, $"Firmware updated from {oldVersion} to {newVersion}", LogSeverity.Info);
    }

    /// <summary>
    /// Increments the cycle count manually
    /// </summary>
    public void IncrementCycleCount(int cycles = 1)
    {
        if (cycles < 0)
            throw new ArgumentException("Cycle count increment must be positive.", nameof(cycles));

        CycleCount += cycles;
    }

    /// <summary>
    /// Checks if the robot requires charging
    /// </summary>
    public bool RequiresCharging(decimal threshold = 20)
    {
        return BatteryLevel.HasValue && BatteryLevel.Value <= threshold;
    }

    /// <summary>
    /// Checks if the robot is currently performing a task
    /// </summary>
    public bool IsPerformingTask()
    {
        return !string.IsNullOrWhiteSpace(CurrentTask);
    }

    /// <summary>
    /// Updates robot specifications
    /// </summary>
    public void UpdateSpecifications(int? maxPayloadKg = null, decimal? reachMeters = null)
    {
        if (maxPayloadKg.HasValue)
        {
            MaxPayloadKg = Ensure.Positive(maxPayloadKg.Value, nameof(maxPayloadKg));
            AddLog(AssetLogType.ConfigurationChanged, $"Max payload updated to {maxPayloadKg}kg", LogSeverity.Info);
        }

        if (reachMeters.HasValue)
        {
            ReachMeters = Ensure.Positive(reachMeters.Value, nameof(reachMeters));
            AddLog(AssetLogType.ConfigurationChanged, $"Reach updated to {reachMeters}m", LogSeverity.Info);
        }
    }

    #endregion

    #region Overridden Behaviors

    /// <summary>
    /// Override to handle robot-specific status changes
    /// </summary>
    public override void ChangeStatus(AssetStatus newStatus)
    {
        // Clear task if robot is being deactivated or put in maintenance
        if ((newStatus == AssetStatus.Inactive || newStatus == AssetStatus.UnderMaintenance) 
            && !string.IsNullOrWhiteSpace(CurrentTask))
        {
            ClearTask();
        }

        base.ChangeStatus(newStatus);
    }

    /// <summary>
    /// Override to record robot-specific maintenance
    /// </summary>
    public override void RecordMaintenance(DateTime maintenanceDate, DateTime? nextScheduledDate = null, string? notes = null)
    {
        // Clear current task during maintenance
        if (!string.IsNullOrWhiteSpace(CurrentTask))
        {
            ClearTask();
        }

        base.RecordMaintenance(maintenanceDate, nextScheduledDate, notes);
    }

    /// <summary>
    /// Override to increment operating hours and handle robot-specific logic
    /// </summary>
    public override void IncrementOperatingHours(int hours)
    {
        base.IncrementOperatingHours(hours);

        // Simulate battery drain for battery-powered robots (rough estimate)
        if (BatteryLevel.HasValue && BatteryLevel.Value > 0)
        {
            var estimatedDrain = hours * 2; // 2% per hour (configurable)
            var newLevel = Math.Max(0, BatteryLevel.Value - estimatedDrain);
            UpdateBatteryLevel(newLevel);
        }
    }

    #endregion
}