using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

/// <summary>
/// Represents an autonomous robot asset.
/// Specialized type of Asset with robot-specific properties and behaviors.
/// </summary>
public class Robot : Asset
{
    #region Robot-Specific Properties

    /// <summary>
    /// Type of robot (Industrial, Collaborative, Mobile, etc.)
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
    /// </summary>
    public decimal? BatteryLevel { get; private set; }

    /// <summary>
    /// Current task being executed
    /// </summary>
    public string? CurrentTask { get; private set; }

    /// <summary>
    /// Total number of cycles completed
    /// </summary>
    public int CycleCount { get; private set; }

    /// <summary>
    /// Firmware version installed
    /// </summary>
    public string? FirmwareVersion { get; private set; }

    /// <summary>
    /// Number of axes of movement
    /// </summary>
    public int? NumberOfAxes { get; private set; }

    /// <summary>
    /// Repeatability accuracy in millimeters
    /// </summary>
    public decimal? RepeatabilityMm { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// EF Core parameterless constructor
    /// </summary>
    protected Robot() : base() { }

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
        : base(name, description, serialNumber, model, manufacturer, assetTag, AssetType.Robot, location, purchaseDate, purchasePrice, criticality)
    {
        RobotType = robotType;
        MaxPayloadKg = Ensure.Positive(maxPayloadKg, nameof(maxPayloadKg));
        ReachMeters = Ensure.Positive(reachMeters, nameof(reachMeters));
        FirmwareVersion = firmwareVersion;
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates a new robot instance
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
        AssetCriticality criticality = AssetCriticality.High,
        string? firmwareVersion = null)
    {
        var robot = new Robot(
            name, description, serialNumber, model, manufacturer, assetTag,
            location, purchaseDate, purchasePrice, criticality,
            robotType, maxPayloadKg, reachMeters, firmwareVersion);

        robot.AddLog(AssetLogType.StatusChange, $"Robot '{name}' registered in system", LogSeverity.Info);
        return robot;
    }

    #endregion

    #region Robot-Specific Behaviors

    /// <summary>
    /// Assigns a task to the robot
    /// </summary>
    public void AssignTask(string task)
    {
        CurrentTask = Ensure.NotEmpty(task, nameof(task));
        
        if (Status == AssetStatus.Idle)
            ChangeStatus(AssetStatus.Running);

        AddLog(AssetLogType.StatusChange, $"Task assigned: {task}", LogSeverity.Info);
    }

    /// <summary>
    /// Marks the current task as complete
    /// </summary>
    public void CompleteTask()
    {
        if (string.IsNullOrWhiteSpace(CurrentTask))
            throw new InvalidOperationException("No task is currently assigned.");

        var completedTask = CurrentTask;
        CurrentTask = null;
        CycleCount++;

        if (Status == AssetStatus.Running)
            ChangeStatus(AssetStatus.Idle);

        AddLog(AssetLogType.StatusChange, $"Task completed: {completedTask}. Total cycles: {CycleCount}", LogSeverity.Info);
    }

    /// <summary>
    /// Updates the robot's battery level
    /// </summary>
    public void UpdateBatteryLevel(decimal level)
    {
        if (level < 0 || level > 100)
            throw new ArgumentException("Battery level must be between 0 and 100.", nameof(level));

        var oldLevel = BatteryLevel;
        BatteryLevel = level;

        // Log based on battery level
        if (level < 10)
        {
            AddLog(AssetLogType.BatteryAlert, $"Critical battery level: {level}%", LogSeverity.Error);
            if (Status == AssetStatus.Running)
                ChangeStatus(AssetStatus.Charging);
        }
        else if (level < 20)
        {
            AddLog(AssetLogType.BatteryAlert, $"Low battery level: {level}%", LogSeverity.Warning);
        }
        else if (oldLevel < 20 && level >= 20)
        {
            AddLog(AssetLogType.BatteryAlert, $"Battery level restored: {level}%", LogSeverity.Info);
        }
    }

    /// <summary>
    /// Starts charging the robot
    /// </summary>
    public void StartCharging()
    {
        if (Status == AssetStatus.Running && !string.IsNullOrWhiteSpace(CurrentTask))
            throw new InvalidOperationException("Cannot charge while task is in progress.");

        ChangeStatus(AssetStatus.Charging);
        CurrentTask = null;
        AddLog(AssetLogType.StatusChange, "Charging started", LogSeverity.Info);
    }

    /// <summary>
    /// Completes charging
    /// </summary>
    public void CompleteCharging()
    {
        if (Status != AssetStatus.Charging)
            throw new InvalidOperationException("Robot is not currently charging.");

        BatteryLevel = 100;
        ChangeStatus(AssetStatus.Idle);
        AddLog(AssetLogType.StatusChange, "Charging completed. Battery at 100%", LogSeverity.Info);
    }

    /// <summary>
    /// Updates the firmware version
    /// </summary>
    public void UpdateFirmware(string newVersion)
    {
        var oldVersion = FirmwareVersion;
        FirmwareVersion = Ensure.NotEmpty(newVersion, nameof(newVersion));
        
        AddLog(AssetLogType.ConfigurationChanged, 
            $"Firmware updated from {oldVersion ?? "N/A"} to {newVersion}", 
            LogSeverity.Info);
    }

    /// <summary>
    /// Sets robot specifications
    /// </summary>
    public void SetSpecifications(int? numberOfAxes = null, decimal? repeatabilityMm = null)
    {
        if (numberOfAxes.HasValue)
        {
            NumberOfAxes = Ensure.Positive(numberOfAxes.Value, nameof(numberOfAxes));
            SetMetadata("NumberOfAxes", numberOfAxes.Value.ToString());
        }

        if (repeatabilityMm.HasValue)
        {
            RepeatabilityMm = Ensure.Positive(repeatabilityMm.Value, nameof(repeatabilityMm));
            SetMetadata("RepeatabilityMm", repeatabilityMm.Value.ToString());
        }
    }

    /// <summary>
    /// Checks if robot requires charging
    /// </summary>
    public bool RequiresCharging(decimal threshold = 20)
    {
        return BatteryLevel.HasValue && BatteryLevel.Value < threshold;
    }

    /// <summary>
    /// Checks if robot is available for task assignment
    /// </summary>
    public bool IsAvailableForTask()
    {
        return IsActive 
            && (Status == AssetStatus.Idle || Status == AssetStatus.Active)
            && string.IsNullOrWhiteSpace(CurrentTask)
            && (!BatteryLevel.HasValue || BatteryLevel.Value > 20);
    }

    /// <summary>
    /// Gets robot utilization percentage
    /// </summary>
    public decimal GetUtilizationPercentage()
    {
        if (OperatingHours == 0)
            return 0;

        var totalHoursSinceInstallation = InstallationDate.HasValue
            ? (DateTime.UtcNow - InstallationDate.Value).TotalHours
            : (DateTime.UtcNow - CreatedAt).TotalHours;

        return totalHoursSinceInstallation > 0
            ? (decimal)(OperatingHours / totalHoursSinceInstallation * 100)
            : 0;
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Override status change to handle robot-specific logic
    /// </summary>
    public override void ChangeStatus(AssetStatus newStatus)
    {
        // Additional validation for robots
        if (newStatus == AssetStatus.Running && RequiresCharging(10))
            throw new InvalidOperationException("Cannot start robot with critically low battery.");

        base.ChangeStatus(newStatus);
    }

    public override string ToString()
    {
        return $"Robot: {Name} ({RobotType}) - {SerialNumber}";
    }

    #endregion
}
