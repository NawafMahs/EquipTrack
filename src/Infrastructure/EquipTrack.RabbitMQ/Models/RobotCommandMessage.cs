using System.Text.Json.Serialization;

namespace EquipTrack.Infrastructure.RabbitMQ.Models;

/// <summary>
/// Represents a command message sent to a robot or machine.
/// </summary>
public sealed class RobotCommandMessage
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
    /// Gets or sets the command type.
    /// </summary>
    [JsonPropertyName("command-type")]
    public string CommandType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command parameters.
    /// </summary>
    [JsonPropertyName("parameters")]
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the priority level of the command.
    /// </summary>
    [JsonPropertyName("priority")]
    public CommandPriority Priority { get; set; } = CommandPriority.Normal;

    /// <summary>
    /// Gets or sets the timestamp when the command was created.
    /// </summary>
    [JsonPropertyName("created-at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the command expires.
    /// </summary>
    [JsonPropertyName("expires-at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the user or system that initiated the command.
    /// </summary>
    [JsonPropertyName("initiated-by")]
    public string InitiatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation identifier for tracking related messages.
    /// </summary>
    [JsonPropertyName("correlation-id")]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets whether the command requires acknowledgment.
    /// </summary>
    [JsonPropertyName("requires-acknowledgment")]
    public bool RequiresAcknowledgment { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    [JsonPropertyName("max-retry-attempts")]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets additional metadata for the command.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Creates a new robot command message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="commandType">The command type.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <param name="parameters">Optional command parameters.</param>
    /// <param name="priority">Command priority.</param>
    /// <returns>A new robot command message.</returns>
    public static RobotCommandMessage Create(
        string robotId,
        string commandType,
        string initiatedBy,
        Dictionary<string, object>? parameters = null,
        CommandPriority priority = CommandPriority.Normal)
    {
        return new RobotCommandMessage
        {
            RobotId = robotId,
            CommandType = commandType,
            InitiatedBy = initiatedBy,
            Parameters = parameters ?? new Dictionary<string, object>(),
            Priority = priority,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5) // Default 5-minute expiration
        };
    }

    /// <summary>
    /// Creates a start production command.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="recipeId">The recipe identifier.</param>
    /// <param name="batchNumber">The batch number.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <returns>A start production command message.</returns>
    public static RobotCommandMessage CreateStartProductionCommand(
        string robotId,
        string recipeId,
        string batchNumber,
        string initiatedBy)
    {
        return Create(
            robotId,
            "start-production",
            initiatedBy,
            new Dictionary<string, object>
            {
                ["recipe-id"] = recipeId,
                ["batch-number"] = batchNumber
            },
            CommandPriority.High);
    }

    /// <summary>
    /// Creates a stop production command.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="reason">The reason for stopping.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <returns>A stop production command message.</returns>
    public static RobotCommandMessage CreateStopProductionCommand(
        string robotId,
        string reason,
        string initiatedBy)
    {
        return Create(
            robotId,
            "stop-production",
            initiatedBy,
            new Dictionary<string, object>
            {
                ["reason"] = reason
            },
            CommandPriority.High);
    }

    /// <summary>
    /// Creates an emergency stop command.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <returns>An emergency stop command message.</returns>
    public static RobotCommandMessage CreateEmergencyStopCommand(
        string robotId,
        string initiatedBy)
    {
        return Create(
            robotId,
            "emergency-stop",
            initiatedBy,
            priority: CommandPriority.Critical);
    }

    /// <summary>
    /// Creates a status request command.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <returns>A status request command message.</returns>
    public static RobotCommandMessage CreateStatusRequestCommand(
        string robotId,
        string initiatedBy)
    {
        return Create(
            robotId,
            "request-status",
            initiatedBy,
            priority: CommandPriority.Low);
    }
}

/// <summary>
/// Defines the priority levels for robot commands.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommandPriority
{
    /// <summary>
    /// Low priority command.
    /// </summary>
    [JsonPropertyName("low")]
    Low = 0,

    /// <summary>
    /// Normal priority command.
    /// </summary>
    [JsonPropertyName("normal")]
    Normal = 1,

    /// <summary>
    /// High priority command.
    /// </summary>
    [JsonPropertyName("high")]
    High = 2,

    /// <summary>
    /// Critical priority command (emergency).
    /// </summary>
    [JsonPropertyName("critical")]
    Critical = 3
}