using System.Text.Json.Serialization;

namespace EquipTrack.Infrastructure.RabbitMQ.Models;

/// <summary>
/// Represents an acknowledgment message for a robot command.
/// </summary>
public sealed class CommandAcknowledgmentMessage
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
    /// Gets or sets the original command message identifier.
    /// </summary>
    [JsonPropertyName("command-message-id")]
    public string CommandMessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original command type.
    /// </summary>
    [JsonPropertyName("command-type")]
    public string CommandType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the command was successfully executed.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the acknowledgment status.
    /// </summary>
    [JsonPropertyName("status")]
    public AcknowledgmentStatus Status { get; set; } = AcknowledgmentStatus.Received;

    /// <summary>
    /// Gets or sets the timestamp when the acknowledgment was created.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the error message if the command failed.
    /// </summary>
    [JsonPropertyName("error-message")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error code if the command failed.
    /// </summary>
    [JsonPropertyName("error-code")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the execution duration in milliseconds.
    /// </summary>
    [JsonPropertyName("execution-duration-ms")]
    public long? ExecutionDurationMs { get; set; }

    /// <summary>
    /// Gets or sets the result data from command execution.
    /// </summary>
    [JsonPropertyName("result-data")]
    public Dictionary<string, object> ResultData { get; set; } = new();

    /// <summary>
    /// Gets or sets the correlation identifier for tracking related messages.
    /// </summary>
    [JsonPropertyName("correlation-id")]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Creates a successful acknowledgment message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="commandMessageId">The original command message identifier.</param>
    /// <param name="commandType">The command type.</param>
    /// <param name="executionDurationMs">Optional execution duration.</param>
    /// <param name="resultData">Optional result data.</param>
    /// <returns>A successful acknowledgment message.</returns>
    public static CommandAcknowledgmentMessage CreateSuccess(
        string robotId,
        string commandMessageId,
        string commandType,
        long? executionDurationMs = null,
        Dictionary<string, object>? resultData = null)
    {
        return new CommandAcknowledgmentMessage
        {
            RobotId = robotId,
            CommandMessageId = commandMessageId,
            CommandType = commandType,
            Success = true,
            Status = AcknowledgmentStatus.Completed,
            ExecutionDurationMs = executionDurationMs,
            ResultData = resultData ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Creates a failed acknowledgment message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="commandMessageId">The original command message identifier.</param>
    /// <param name="commandType">The command type.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="errorCode">Optional error code.</param>
    /// <returns>A failed acknowledgment message.</returns>
    public static CommandAcknowledgmentMessage CreateFailure(
        string robotId,
        string commandMessageId,
        string commandType,
        string errorMessage,
        string? errorCode = null)
    {
        return new CommandAcknowledgmentMessage
        {
            RobotId = robotId,
            CommandMessageId = commandMessageId,
            CommandType = commandType,
            Success = false,
            Status = AcknowledgmentStatus.Failed,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Creates a received acknowledgment message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="commandMessageId">The original command message identifier.</param>
    /// <param name="commandType">The command type.</param>
    /// <returns>A received acknowledgment message.</returns>
    public static CommandAcknowledgmentMessage CreateReceived(
        string robotId,
        string commandMessageId,
        string commandType)
    {
        return new CommandAcknowledgmentMessage
        {
            RobotId = robotId,
            CommandMessageId = commandMessageId,
            CommandType = commandType,
            Success = true,
            Status = AcknowledgmentStatus.Received
        };
    }

    /// <summary>
    /// Creates a processing acknowledgment message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="commandMessageId">The original command message identifier.</param>
    /// <param name="commandType">The command type.</param>
    /// <returns>A processing acknowledgment message.</returns>
    public static CommandAcknowledgmentMessage CreateProcessing(
        string robotId,
        string commandMessageId,
        string commandType)
    {
        return new CommandAcknowledgmentMessage
        {
            RobotId = robotId,
            CommandMessageId = commandMessageId,
            CommandType = commandType,
            Success = true,
            Status = AcknowledgmentStatus.Processing
        };
    }

    /// <summary>
    /// Creates a timeout acknowledgment message.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="commandMessageId">The original command message identifier.</param>
    /// <param name="commandType">The command type.</param>
    /// <returns>A timeout acknowledgment message.</returns>
    public static CommandAcknowledgmentMessage CreateTimeout(
        string robotId,
        string commandMessageId,
        string commandType)
    {
        return new CommandAcknowledgmentMessage
        {
            RobotId = robotId,
            CommandMessageId = commandMessageId,
            CommandType = commandType,
            Success = false,
            Status = AcknowledgmentStatus.Timeout,
            ErrorMessage = "Command execution timed out"
        };
    }
}

/// <summary>
/// Defines the acknowledgment status for robot commands.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AcknowledgmentStatus
{
    /// <summary>
    /// Command has been received.
    /// </summary>
    [JsonPropertyName("received")]
    Received = 0,

    /// <summary>
    /// Command is being processed.
    /// </summary>
    [JsonPropertyName("processing")]
    Processing = 1,

    /// <summary>
    /// Command has been completed successfully.
    /// </summary>
    [JsonPropertyName("completed")]
    Completed = 2,

    /// <summary>
    /// Command execution failed.
    /// </summary>
    [JsonPropertyName("failed")]
    Failed = 3,

    /// <summary>
    /// Command execution timed out.
    /// </summary>
    [JsonPropertyName("timeout")]
    Timeout = 4,

    /// <summary>
    /// Command was rejected.
    /// </summary>
    [JsonPropertyName("rejected")]
    Rejected = 5
}