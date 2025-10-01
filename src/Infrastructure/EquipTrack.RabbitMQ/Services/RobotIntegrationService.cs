using EquipTrack.Application.Common.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Infrastructure.RabbitMQ.Abstractions;
using EquipTrack.Infrastructure.RabbitMQ.Models;
using EquipTrack.RabbitMQ.Models;
using Microsoft.Extensions.Logging;

namespace EquipTrack.Infrastructure.RabbitMQ.Services;

/// <summary>
/// Service that integrates robot messaging with domain entities and repository operations.
/// Demonstrates production-ready usage of the RabbitMQ messaging layer.
/// </summary>
public sealed class RobotIntegrationService : IDisposable
{
    private readonly IRobotMessagingService _messagingService;
    private readonly ILogger<RobotIntegrationService> _logger;
    private bool _disposed;

    /// <summary>
    /// Event raised when a robot status update is processed and mapped to domain entities.
    /// </summary>
    public event EventHandler<RobotStatusProcessedEventArgs>? RobotStatusProcessed;

    /// <summary>
    /// Event raised when a robot command is acknowledged.
    /// </summary>
    public event EventHandler<RobotCommandAcknowledgedEventArgs>? RobotCommandAcknowledged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RobotIntegrationService"/> class.
    /// </summary>
    /// <param name="messagingService">The robot messaging service.</param>
    /// <param name="logger">The logger instance.</param>
    public RobotIntegrationService(
        IRobotMessagingService messagingService,
        ILogger<RobotIntegrationService> logger)
    {
        _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Subscribe to messaging events
        _messagingService.StatusReceived += OnRobotStatusReceived;
        _messagingService.CommandAcknowledged += OnRobotCommandAcknowledged;
    }

    /// <summary>
    /// Sends a start production command to a robot and handles the response.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="recipeId">The recipe identifier.</param>
    /// <param name="batchNumber">The batch number.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the command acknowledgment.</returns>
    public async Task<Result<RobotProductionStartResult>> StartProductionAsync(
        string robotId,
        string recipeId,
        string batchNumber,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting production on robot {RobotId} with recipe {RecipeId} and batch {BatchNumber}",
                robotId, recipeId, batchNumber);

            // Create and send the command
            var command = RobotCommandMessage.CreateStartProductionCommand(robotId, recipeId, batchNumber, initiatedBy);
            
            // Send command and wait for acknowledgment
            var ackResult = await _messagingService.SendCommandAndWaitForAckAsync(command, 30, cancellationToken);
            if (!ackResult.IsSuccess)
            {
                _logger.LogError("Failed to start production on robot {RobotId}: {Errors}",
                    robotId, string.Join(", ", ackResult.Errors));
                
                return Result<RobotProductionStartResult>.Error(ackResult.Errors.ToArray());
            }

            var acknowledgment = ackResult.Value;
            var result = new RobotProductionStartResult
            {
                RobotId = robotId,
                RecipeId = recipeId,
                BatchNumber = batchNumber,
                CommandId = command.MessageId,
                Success = acknowledgment.Success,
                AcknowledgmentStatus = acknowledgment.Status,
                ErrorMessage = acknowledgment.ErrorMessage,
                ExecutionDurationMs = acknowledgment.ExecutionDurationMs,
                Timestamp = acknowledgment.Timestamp
            };

            if (acknowledgment.Success)
            {
                _logger.LogInformation("Successfully started production on robot {RobotId} for batch {BatchNumber}",
                    robotId, batchNumber);
            }
            else
            {
                _logger.LogWarning("Production start failed on robot {RobotId}: {ErrorMessage}",
                    robotId, acknowledgment.ErrorMessage);
            }

            return Result<RobotProductionStartResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting production on robot {RobotId}", robotId);
            return Result<RobotProductionStartResult>.Error($"Failed to start production: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends a stop production command to a robot.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="reason">The reason for stopping.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the command acknowledgment.</returns>
    public async Task<Result<RobotProductionStopResult>> StopProductionAsync(
        string robotId,
        string reason,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Stopping production on robot {RobotId} with reason: {Reason}",
                robotId, reason);

            var command = RobotCommandMessage.CreateStopProductionCommand(robotId, reason, initiatedBy);
            
            var ackResult = await _messagingService.SendCommandAndWaitForAckAsync(command, 30, cancellationToken);
            if (!ackResult.IsSuccess)
            {
                _logger.LogError("Failed to stop production on robot {RobotId}: {Errors}",
                    robotId, string.Join(", ", ackResult.Errors));
                
                return Result<RobotProductionStopResult>.Error(ackResult.Errors.ToArray());
            }

            var acknowledgment = ackResult.Value;
            var result = new RobotProductionStopResult
            {
                RobotId = robotId,
                Reason = reason,
                CommandId = command.MessageId,
                Success = acknowledgment.Success,
                AcknowledgmentStatus = acknowledgment.Status,
                ErrorMessage = acknowledgment.ErrorMessage,
                ExecutionDurationMs = acknowledgment.ExecutionDurationMs,
                Timestamp = acknowledgment.Timestamp
            };

            if (acknowledgment.Success)
            {
                _logger.LogInformation("Successfully stopped production on robot {RobotId}", robotId);
            }
            else
            {
                _logger.LogWarning("Production stop failed on robot {RobotId}: {ErrorMessage}",
                    robotId, acknowledgment.ErrorMessage);
            }

            return Result<RobotProductionStopResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping production on robot {RobotId}", robotId);
            return Result<RobotProductionStopResult>.Error($"Failed to stop production: {ex.Message}");
        }
    }

    /// <summary>
    /// Requests current status from a robot and waits for the response.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the robot status information.</returns>
    public async Task<Result<RobotStatusInfo>> GetRobotStatusAsync(
        string robotId,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Requesting status from robot {RobotId}", robotId);

            var statusResult = await _messagingService.RequestStatusAndWaitAsync(robotId, initiatedBy, 30, cancellationToken);
            if (!statusResult.IsSuccess)
            {
                _logger.LogError("Failed to get status from robot {RobotId}: {Errors}",
                    robotId, string.Join(", ", statusResult.Errors));
                
                return Result<RobotStatusInfo>.Error(statusResult.Errors.ToArray());
            }

            var statusMessage = statusResult.Value;
            var statusInfo = MapToRobotStatusInfo(statusMessage);

            _logger.LogDebug("Received status {Status} from robot {RobotId}", statusInfo.Status, robotId);

            return Result<RobotStatusInfo>.Success(statusInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting status from robot {RobotId}", robotId);
            return Result<RobotStatusInfo>.Error($"Failed to get robot status: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends an emergency stop command to a robot.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the emergency stop.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> EmergencyStopAsync(
        string robotId,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("Initiating emergency stop for robot {RobotId} by {InitiatedBy}",
                robotId, initiatedBy);

            var result = await _messagingService.EmergencyStopAsync(robotId, initiatedBy, cancellationToken);
            
            if (result.IsSuccess)
            {
                _logger.LogWarning("Emergency stop command sent to robot {RobotId}", robotId);
            }
            else
            {
                _logger.LogError("Failed to send emergency stop to robot {RobotId}: {Errors}",
                    robotId, string.Join(", ", result.Errors));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending emergency stop to robot {RobotId}", robotId);
            return Result.Error($"Failed to send emergency stop: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            // Unsubscribe from events
            _messagingService.StatusReceived -= OnRobotStatusReceived;
            _messagingService.CommandAcknowledged -= OnRobotCommandAcknowledged;

            _logger.LogInformation("Robot integration service disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during robot integration service disposal");
        }
    }

    private void OnRobotStatusReceived(object? sender, RobotStatusMessage statusMessage)
    {
        try
        {
            var statusInfo = MapToRobotStatusInfo(statusMessage);
            var eventArgs = new RobotStatusProcessedEventArgs(statusInfo, statusMessage);
            
            RobotStatusProcessed?.Invoke(this, eventArgs);

            _logger.LogDebug("Processed status update from robot {RobotId}: {Status}",
                statusMessage.RobotId, statusMessage.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing robot status message from {RobotId}",
                statusMessage.RobotId);
        }
    }

    private void OnRobotCommandAcknowledged(object? sender, CommandAcknowledgmentMessage acknowledgment)
    {
        try
        {
            var eventArgs = new RobotCommandAcknowledgedEventArgs(acknowledgment);
            RobotCommandAcknowledged?.Invoke(this, eventArgs);

            _logger.LogDebug("Processed command acknowledgment from robot {RobotId} for command {CommandId}",
                acknowledgment.RobotId, acknowledgment.CommandMessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing command acknowledgment from robot {RobotId}",
                acknowledgment.RobotId);
        }
    }

    private static RobotStatusInfo MapToRobotStatusInfo(RobotStatusMessage statusMessage)
    {
        return new RobotStatusInfo
        {
            RobotId = statusMessage.RobotId.ToString(),
            Status = statusMessage.Status.ToString().ToLowerInvariant(),
            Timestamp = statusMessage.Timestamp,
            SensorReadings = statusMessage.SensorReadings?.ToDictionary(sr => sr.SensorId, sr => sr.Value) ?? new Dictionary<string, decimal>(),
            ErrorCodes = statusMessage.ErrorCodes ?? new List<string>(),
            Warnings = statusMessage.Warnings ?? new List<string>(),
            BatteryLevel = statusMessage.BatteryLevel,
            OperatingHours = statusMessage.OperatingHours ?? 0m,
            FirmwareVersion = statusMessage.FirmwareVersion,
            ProductionInfo = statusMessage.ProductionInfo != null ? new ProductionStatusInfo
            {
                RecipeId = statusMessage.ProductionInfo.RecipeId,
                BatchNumber = statusMessage.ProductionInfo.BatchNumber,
                StartedAt = statusMessage.ProductionInfo.StartedAt,
                EstimatedCompletion = statusMessage.ProductionInfo.EstimatedCompletion,
                ProgressPercentage = statusMessage.ProductionInfo.ProgressPercentage ?? 0m,
                CurrentStep = statusMessage.ProductionInfo.CurrentStep,
                TotalSteps = statusMessage.ProductionInfo.TotalSteps ?? 0,
                CurrentStepNumber = statusMessage.ProductionInfo.CurrentStepNumber ?? 0
            } : null,
            NetworkInfo = statusMessage.NetworkInfo != null ? new NetworkStatusInfo
            {
                SignalStrength = (decimal)(statusMessage.NetworkInfo.SignalStrength ?? 0),
                IpAddress = statusMessage.NetworkInfo.IpAddress,
                ConnectionType = statusMessage.NetworkInfo.ConnectionType,
                LastCommunication = statusMessage.NetworkInfo.LastCommunication ?? DateTime.UtcNow
            } : null
        };
    }
}

/// <summary>
/// Result of a robot production start operation.
/// </summary>
public sealed class RobotProductionStartResult
{
    public string RobotId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string CommandId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public AcknowledgmentStatus AcknowledgmentStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public long? ExecutionDurationMs { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Result of a robot production stop operation.
/// </summary>
public sealed class RobotProductionStopResult
{
    public string RobotId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string CommandId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public AcknowledgmentStatus AcknowledgmentStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public long? ExecutionDurationMs { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Domain-friendly representation of robot status information.
/// </summary>
public sealed class RobotStatusInfo
{
    public string RobotId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, decimal> SensorReadings { get; set; } = new();
    public List<string> ErrorCodes { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public decimal? BatteryLevel { get; set; }
    public decimal OperatingHours { get; set; }
    public string? FirmwareVersion { get; set; }
    public ProductionStatusInfo? ProductionInfo { get; set; }
    public NetworkStatusInfo? NetworkInfo { get; set; }
}

/// <summary>
/// Production status information from a robot.
/// </summary>
public sealed class ProductionStatusInfo
{
    public string? RecipeId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public int CurrentStepNumber { get; set; }
}

/// <summary>
/// Network status information from a robot.
/// </summary>
public sealed class NetworkStatusInfo
{
    public decimal SignalStrength { get; set; }
    public string? IpAddress { get; set; }
    public string? ConnectionType { get; set; }
    public DateTime LastCommunication { get; set; }
}

/// <summary>
/// Event arguments for robot status processed events.
/// </summary>
public sealed class RobotStatusProcessedEventArgs : EventArgs
{
    public RobotStatusInfo StatusInfo { get; }
    public RobotStatusMessage OriginalMessage { get; }

    public RobotStatusProcessedEventArgs(RobotStatusInfo statusInfo, RobotStatusMessage originalMessage)
    {
        StatusInfo = statusInfo;
        OriginalMessage = originalMessage;
    }
}

/// <summary>
/// Event arguments for robot command acknowledged events.
/// </summary>
public sealed class RobotCommandAcknowledgedEventArgs : EventArgs
{
    public CommandAcknowledgmentMessage Acknowledgment { get; }

    public RobotCommandAcknowledgedEventArgs(CommandAcknowledgmentMessage acknowledgment)
    {
        Acknowledgment = acknowledgment;
    }
}