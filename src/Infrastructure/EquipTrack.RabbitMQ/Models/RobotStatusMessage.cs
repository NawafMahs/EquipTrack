using System.Text.Json.Serialization;

namespace EquipTrack.Infrastructure.RabbitMQ.Models;

/// <summary>
/// Represents a status update message received from a robot or machine.
/// </summary>
public sealed class RobotStatusMessage
{
    /// <summary>
    /// Gets or sets the unique message identifier.
    /// </summary>
    [JsonPropertyName("message-id")]
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the robot or machine identifier.
    /// </summary>
    [JsonPropertyName("robot-id")]
    public string RobotId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current operational status.
    /// </summary>
    [JsonPropertyName("status")]
    public RobotOperationalStatus Status { get; set; } = RobotOperationalStatus.Unknown;

    /// <summary>
    /// Gets or sets the timestamp when the status was recorded.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the sensor readings from the robot.
    /// </summary>
    [JsonPropertyName("sensor-readings")]
    public Dictionary<string, decimal> SensorReadings { get; set; } = new();

    /// <summary>
    /// Gets or sets the current production information.
    /// </summary>
    [JsonPropertyName("production-info")]
    public ProductionInfo? ProductionInfo { get; set; }

    /// <summary>
    /// Gets or sets any error codes or messages.
    /// </summary>
    [JsonPropertyName("error-codes")]
    public List<string> ErrorCodes { get; set; } = new();

    /// <summary>
    /// Gets or sets any warning messages.
    /// </summary>
    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Gets or sets the battery level (if applicable).
    /// </summary>
    [JsonPropertyName("battery-level")]
    public decimal? BatteryLevel { get; set; }

    /// <summary>
    /// Gets or sets the total operating hours.
    /// </summary>
    [JsonPropertyName("operating-hours")]
    public decimal OperatingHours { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier for tracking related messages.
    /// </summary>
    [JsonPropertyName("correlation-id")]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the firmware version.
    /// </summary>
    [JsonPropertyName("firmware-version")]
    public string? FirmwareVersion { get; set; }

    /// <summary>
    /// Gets or sets the network connectivity information.
    /// </summary>
    [JsonPropertyName("network-info")]
    public NetworkInfo? NetworkInfo { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Creates a basic status message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="status">The operational status.</param>
    /// <returns>A new robot status message.</returns>
    public static RobotStatusMessage Create(string robotId, RobotOperationalStatus status)
    {
        return new RobotStatusMessage
        {
            RobotId = robotId,
            Status = status
        };
    }

    /// <summary>
    /// Creates a status message with sensor readings.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="status">The operational status.</param>
    /// <param name="sensorReadings">The sensor readings.</param>
    /// <returns>A new robot status message with sensor data.</returns>
    public static RobotStatusMessage CreateWithSensorData(
        string robotId,
        RobotOperationalStatus status,
        Dictionary<string, decimal> sensorReadings)
    {
        return new RobotStatusMessage
        {
            RobotId = robotId,
            Status = status,
            SensorReadings = sensorReadings
        };
    }

    /// <summary>
    /// Creates an error status message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="errorCodes">The error codes.</param>
    /// <returns>A new robot status message indicating error.</returns>
    public static RobotStatusMessage CreateErrorStatus(
        string robotId,
        params string[] errorCodes)
    {
        return new RobotStatusMessage
        {
            RobotId = robotId,
            Status = RobotOperationalStatus.Error,
            ErrorCodes = errorCodes.ToList()
        };
    }
}

/// <summary>
/// Represents production information from a robot.
/// </summary>
public sealed class ProductionInfo
{
    /// <summary>
    /// Gets or sets the current recipe identifier.
    /// </summary>
    [JsonPropertyName("recipe-id")]
    public string? RecipeId { get; set; }

    /// <summary>
    /// Gets or sets the current batch number.
    /// </summary>
    [JsonPropertyName("batch-number")]
    public string? BatchNumber { get; set; }

    /// <summary>
    /// Gets or sets the production start time.
    /// </summary>
    [JsonPropertyName("started-at")]
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the estimated completion time.
    /// </summary>
    [JsonPropertyName("estimated-completion")]
    public DateTime? EstimatedCompletion { get; set; }

    /// <summary>
    /// Gets or sets the production progress percentage.
    /// </summary>
    [JsonPropertyName("progress-percentage")]
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Gets or sets the current production step.
    /// </summary>
    [JsonPropertyName("current-step")]
    public string? CurrentStep { get; set; }

    /// <summary>
    /// Gets or sets the total number of steps.
    /// </summary>
    [JsonPropertyName("total-steps")]
    public int TotalSteps { get; set; }

    /// <summary>
    /// Gets or sets the current step number.
    /// </summary>
    [JsonPropertyName("current-step-number")]
    public int CurrentStepNumber { get; set; }
}

/// <summary>
/// Represents network connectivity information.
/// </summary>
public sealed class NetworkInfo
{
    /// <summary>
    /// Gets or sets the signal strength percentage.
    /// </summary>
    [JsonPropertyName("signal-strength")]
    public decimal SignalStrength { get; set; }

    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    [JsonPropertyName("ip-address")]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the connection type.
    /// </summary>
    [JsonPropertyName("connection-type")]
    public string? ConnectionType { get; set; }

    /// <summary>
    /// Gets or sets the last communication timestamp.
    /// </summary>
    [JsonPropertyName("last-communication")]
    public DateTime LastCommunication { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Defines the operational status of a robot or machine.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RobotOperationalStatus
{
    /// <summary>
    /// Status is unknown.
    /// </summary>
    [JsonPropertyName("unknown")]
    Unknown = 0,

    /// <summary>
    /// Robot is offline.
    /// </summary>
    [JsonPropertyName("offline")]
    Offline = 1,

    /// <summary>
    /// Robot is online and idle.
    /// </summary>
    [JsonPropertyName("idle")]
    Idle = 2,

    /// <summary>
    /// Robot is starting up.
    /// </summary>
    [JsonPropertyName("starting")]
    Starting = 3,

    /// <summary>
    /// Robot is actively producing.
    /// </summary>
    [JsonPropertyName("producing")]
    Producing = 4,

    /// <summary>
    /// Robot is paused.
    /// </summary>
    [JsonPropertyName("paused")]
    Paused = 5,

    /// <summary>
    /// Robot is stopping.
    /// </summary>
    [JsonPropertyName("stopping")]
    Stopping = 6,

    /// <summary>
    /// Robot is in error state.
    /// </summary>
    [JsonPropertyName("error")]
    Error = 7,

    /// <summary>
    /// Robot is under maintenance.
    /// </summary>
    [JsonPropertyName("maintenance")]
    Maintenance = 8,

    /// <summary>
    /// Robot is in emergency stop state.
    /// </summary>
    [JsonPropertyName("emergency-stop")]
    EmergencyStop = 9
}