using EquipTrack.Core.SharedKernel;
using EquipTrack.Infrastructure.RabbitMQ.Abstractions;
using EquipTrack.Infrastructure.RabbitMQ.Configuration;
using EquipTrack.Infrastructure.RabbitMQ.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace EquipTrack.Infrastructure.RabbitMQ.Services;

/// <summary>
/// Service for handling robot communication via RabbitMQ messaging with Result pattern support.
/// </summary>
public sealed class RobotMessagingService : IRobotMessagingService
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RobotMessagingService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<CommandAcknowledgmentMessage>> _pendingCommands = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<RobotStatusMessage>> _pendingStatusRequests = new();
    
    private IChannel? _publishChannel;
    private IChannel? _consumeChannel;
    private bool _initialized;
    private bool _disposed;

    /// <inheritdoc />
    public event EventHandler<RobotStatusMessage>? StatusReceived;

    /// <inheritdoc />
    public event EventHandler<CommandAcknowledgmentMessage>? CommandAcknowledged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RobotMessagingService"/> class.
    /// </summary>
    /// <param name="connectionFactory">The RabbitMQ connection factory.</param>
    /// <param name="options">The RabbitMQ configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public RobotMessagingService(
        IRabbitMqConnectionFactory connectionFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<RobotMessagingService> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <inheritdoc />
    public async Task<Result> InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return Result.Error("Service has been disposed");

        if (_initialized)
            return Result.Success();

        try
        {
            // Create channels for publishing and consuming
            var publishChannelResult = await _connectionFactory.CreateChannelAsync(cancellationToken);
            if (!publishChannelResult.IsSuccess)
            {
                return Result.Error($"Failed to create publish channel: {string.Join(", ", publishChannelResult.Errors)}");
            }

            var consumeChannelResult = await _connectionFactory.CreateChannelAsync(cancellationToken);
            if (!consumeChannelResult.IsSuccess)
            {
                return Result.Error($"Failed to create consume channel: {string.Join(", ", consumeChannelResult.Errors)}");
            }

            _publishChannel = publishChannelResult.Value;
            _consumeChannel = consumeChannelResult.Value;

            // Set up exchanges and queues
            var setupResult = await SetupTopologyAsync(cancellationToken);
            if (!setupResult.IsSuccess)
            {
                return setupResult;
            }

            // Start consuming messages
            var consumeResult = await StartConsumingAsync(cancellationToken);
            if (!consumeResult.IsSuccess)
            {
                return consumeResult;
            }

            _initialized = true;
            _logger.LogInformation("Robot messaging service initialized successfully");
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize robot messaging service");
            return Result.Error($"Initialization failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> SendCommandAsync(RobotCommandMessage command, CancellationToken cancellationToken = default)
    {
        if (!_initialized)
            return Result.Error("Service not initialized");

        if (_disposed)
            return Result.Error("Service has been disposed");

        if (_publishChannel == null)
            return Result.Error("Publish channel not available");

        try
        {
            var messageBody = JsonSerializer.SerializeToUtf8Bytes(command, _jsonOptions);
            var routingKey = $"command.{command.RobotId}.{command.CommandType}";

            var properties = new BasicProperties
            {
                MessageId = command.MessageId,
                CorrelationId = command.CorrelationId,
                Timestamp = new AmqpTimestamp(((DateTimeOffset)command.CreatedAt).ToUnixTimeSeconds()),
                Expiration = command.ExpiresAt?.Subtract(DateTime.UtcNow).TotalMilliseconds.ToString("F0"),
                Priority = (byte)command.Priority,
                Persistent = _options.EnableMessagePersistence,
                Headers = new Dictionary<string, object?>
                {
                    ["robot-id"] = command.RobotId,
                    ["command-type"] = command.CommandType,
                    ["initiated-by"] = command.InitiatedBy
                }
            };

            await _publishChannel.BasicPublishAsync(
                exchange: _options.RobotCommandExchange.Name,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: messageBody,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Sent command {CommandType} to robot {RobotId} with message ID {MessageId}",
                command.CommandType, command.RobotId, command.MessageId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send command {CommandType} to robot {RobotId}",
                command.CommandType, command.RobotId);
            
            return Result.Error($"Failed to send command: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<CommandAcknowledgmentMessage>> SendCommandAndWaitForAckAsync(
        RobotCommandMessage command,
        int timeoutSeconds = 30,
        CancellationToken cancellationToken = default)
    {
        if (!command.RequiresAcknowledgment)
        {
            var sendResult = await SendCommandAsync(command, cancellationToken);
            if (!sendResult.IsSuccess)
                return Result<CommandAcknowledgmentMessage>.Error(sendResult.Errors.ToArray());

            // Create a synthetic acknowledgment for commands that don't require one
            var syntheticAck = CommandAcknowledgmentMessage.CreateReceived(
                command.RobotId, command.MessageId, command.CommandType);
            
            return Result<CommandAcknowledgmentMessage>.Success(syntheticAck);
        }

        var tcs = new TaskCompletionSource<CommandAcknowledgmentMessage>();
        _pendingCommands[command.MessageId] = tcs;

        try
        {
            var sendResult = await SendCommandAsync(command, cancellationToken);
            if (!sendResult.IsSuccess)
            {
                _pendingCommands.TryRemove(command.MessageId, out _);
                return Result<CommandAcknowledgmentMessage>.Error(sendResult.Errors.ToArray());
            }

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                var acknowledgment = await tcs.Task.WaitAsync(combinedCts.Token);
                return Result<CommandAcknowledgmentMessage>.Success(acknowledgment);
            }
            catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
            {
                _logger.LogWarning("Command {CommandType} to robot {RobotId} timed out after {Timeout} seconds",
                    command.CommandType, command.RobotId, timeoutSeconds);
                
                return Result<CommandAcknowledgmentMessage>.Error($"Command timed out after {timeoutSeconds} seconds");
            }
        }
        finally
        {
            _pendingCommands.TryRemove(command.MessageId, out _);
        }
    }

    /// <inheritdoc />
    public async Task<Result> StartProductionAsync(
        string robotId,
        string recipeId,
        string batchNumber,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        var command = RobotCommandMessage.CreateStartProductionCommand(robotId, recipeId, batchNumber, initiatedBy);
        return await SendCommandAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> StopProductionAsync(
        string robotId,
        string reason,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        var command = RobotCommandMessage.CreateStopProductionCommand(robotId, reason, initiatedBy);
        return await SendCommandAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> EmergencyStopAsync(
        string robotId,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        var command = RobotCommandMessage.CreateEmergencyStopCommand(robotId, initiatedBy);
        return await SendCommandAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> RequestStatusAsync(
        string robotId,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        var command = RobotCommandMessage.CreateStatusRequestCommand(robotId, initiatedBy);
        return await SendCommandAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<RobotStatusMessage>> RequestStatusAndWaitAsync(
        string robotId,
        string initiatedBy,
        int timeoutSeconds = 30,
        CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<RobotStatusMessage>();
        _pendingStatusRequests[requestId] = tcs;

        try
        {
            var command = RobotCommandMessage.CreateStatusRequestCommand(robotId, initiatedBy);
            command.CorrelationId = requestId;

            var sendResult = await SendCommandAsync(command, cancellationToken);
            if (!sendResult.IsSuccess)
            {
                _pendingStatusRequests.TryRemove(requestId, out _);
                return Result<RobotStatusMessage>.Error(sendResult.Errors.ToArray());
            }

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                var statusMessage = await tcs.Task.WaitAsync(combinedCts.Token);
                return Result<RobotStatusMessage>.Success(statusMessage);
            }
            catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
            {
                _logger.LogWarning("Status request for robot {RobotId} timed out after {Timeout} seconds",
                    robotId, timeoutSeconds);
                
                return Result<RobotStatusMessage>.Error($"Status request timed out after {timeoutSeconds} seconds");
            }
        }
        finally
        {
            _pendingStatusRequests.TryRemove(requestId, out _);
        }
    }

    /// <inheritdoc />
    public async Task<Result> PublishStatusAsync(RobotStatusMessage statusMessage, CancellationToken cancellationToken = default)
    {
        if (!_initialized)
            return Result.Error("Service not initialized");

        if (_disposed)
            return Result.Error("Service has been disposed");

        if (_publishChannel == null)
            return Result.Error("Publish channel not available");

        try
        {
            var messageBody = JsonSerializer.SerializeToUtf8Bytes(statusMessage, _jsonOptions);
            var routingKey = $"status.{statusMessage.RobotId}.{statusMessage.Status.ToString().ToLowerInvariant()}";

            var properties = new BasicProperties
            {
                MessageId = statusMessage.MessageId,
                CorrelationId = statusMessage.CorrelationId,
                Timestamp = new AmqpTimestamp(((DateTimeOffset)statusMessage.Timestamp).ToUnixTimeSeconds()),
                Persistent = _options.EnableMessagePersistence,
                Headers = new Dictionary<string, object?>
                {
                    ["robot-id"] = statusMessage.RobotId,
                    ["status"] = statusMessage.Status.ToString().ToLowerInvariant()
                }
            };

            await _publishChannel.BasicPublishAsync(
                exchange: _options.RobotStatusExchange.Name,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: messageBody,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Published status {Status} for robot {RobotId} with message ID {MessageId}",
                statusMessage.Status, statusMessage.RobotId, statusMessage.MessageId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish status for robot {RobotId}",
                statusMessage.RobotId);
            
            return Result.Error($"Failed to publish status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> PublishAcknowledgmentAsync(CommandAcknowledgmentMessage acknowledgment, CancellationToken cancellationToken = default)
    {
        if (!_initialized)
            return Result.Error("Service not initialized");

        if (_disposed)
            return Result.Error("Service has been disposed");

        if (_publishChannel == null)
            return Result.Error("Publish channel not available");

        try
        {
            var messageBody = JsonSerializer.SerializeToUtf8Bytes(acknowledgment, _jsonOptions);
            var routingKey = $"command.ack.{acknowledgment.RobotId}.{acknowledgment.CommandType}";

            var properties = new BasicProperties
            {
                MessageId = acknowledgment.MessageId,
                CorrelationId = acknowledgment.CorrelationId,
                Timestamp = new AmqpTimestamp(((DateTimeOffset)acknowledgment.Timestamp).ToUnixTimeSeconds()),
                Persistent = _options.EnableMessagePersistence,
                Headers = new Dictionary<string, object?>
                {
                    ["robot-id"] = acknowledgment.RobotId,
                    ["command-message-id"] = acknowledgment.CommandMessageId,
                    ["success"] = acknowledgment.Success
                }
            };

            await _publishChannel.BasicPublishAsync(
                exchange: _options.RobotCommandExchange.Name,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: messageBody,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Published acknowledgment for command {CommandMessageId} from robot {RobotId}",
                acknowledgment.CommandMessageId, acknowledgment.RobotId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish acknowledgment for robot {RobotId}",
                acknowledgment.RobotId);
            
            return Result.Error($"Failed to publish acknowledgment: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> IsHealthyAsync()
    {
        try
        {
            var connectionHealthy = _connectionFactory.IsHealthy();
            if (!connectionHealthy.IsSuccess || !connectionHealthy.Value)
            {
                return Result<bool>.Success(false);
            }

            var channelsHealthy = _publishChannel?.IsOpen == true && _consumeChannel?.IsOpen == true;
            return Result<bool>.Success(_initialized && channelsHealthy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health status");
            return Result<bool>.Error($"Health check failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Dictionary<string, object>>> GetConnectionStatusAsync()
    {
        try
        {
            var connectionStatus = _connectionFactory.GetConnectionStatus();
            if (!connectionStatus.IsSuccess)
            {
                return Result<Dictionary<string, object>>.Error(connectionStatus.Errors.ToArray());
            }

            var status = connectionStatus.Value;
            status["initialized"] = _initialized;
            status["publish-channel-open"] = _publishChannel?.IsOpen ?? false;
            status["consume-channel-open"] = _consumeChannel?.IsOpen ?? false;
            status["pending-commands"] = _pendingCommands.Count;
            status["pending-status-requests"] = _pendingStatusRequests.Count;

            return Result<Dictionary<string, object>>.Success(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting connection status");
            return Result<Dictionary<string, object>>.Error($"Failed to get connection status: {ex.Message}");
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
            // Cancel all pending operations
            foreach (var tcs in _pendingCommands.Values)
            {
                tcs.TrySetCanceled();
            }
            _pendingCommands.Clear();

            foreach (var tcs in _pendingStatusRequests.Values)
            {
                tcs.TrySetCanceled();
            }
            _pendingStatusRequests.Clear();

            // Close channels
            _publishChannel?.CloseAsync().GetAwaiter().GetResult();
            _publishChannel?.Dispose();

            _consumeChannel?.CloseAsync().GetAwaiter().GetResult();
            _consumeChannel?.Dispose();

            _logger.LogInformation("Robot messaging service disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during robot messaging service disposal");
        }
    }

    private async Task<Result> SetupTopologyAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_publishChannel == null)
                return Result.Error("Publish channel not available");

            // Declare exchanges
            await _publishChannel.ExchangeDeclareAsync(
                exchange: _options.RobotCommandExchange.Name,
                type: _options.RobotCommandExchange.Type,
                durable: _options.RobotCommandExchange.Durable,
                autoDelete: _options.RobotCommandExchange.AutoDelete,
                arguments: _options.RobotCommandExchange.Arguments,
                cancellationToken: cancellationToken);

            await _publishChannel.ExchangeDeclareAsync(
                exchange: _options.RobotStatusExchange.Name,
                type: _options.RobotStatusExchange.Type,
                durable: _options.RobotStatusExchange.Durable,
                autoDelete: _options.RobotStatusExchange.AutoDelete,
                arguments: _options.RobotStatusExchange.Arguments,
                cancellationToken: cancellationToken);

            // Declare dead letter exchange if enabled
            if (_options.EnableDeadLetterExchange)
            {
                await _publishChannel.ExchangeDeclareAsync(
                    exchange: _options.DeadLetterExchangeName,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);
            }

            // Declare queues
            var queueArgs = new Dictionary<string, object?>();
            if (_options.EnableDeadLetterExchange)
            {
                queueArgs["x-dead-letter-exchange"] = _options.DeadLetterExchangeName;
            }
            if (_options.MessageTtlMilliseconds > 0)
            {
                queueArgs["x-message-ttl"] = _options.MessageTtlMilliseconds;
            }

            await _publishChannel.QueueDeclareAsync(
                queue: _options.CommandAckQueue.Name,
                durable: _options.CommandAckQueue.Durable,
                exclusive: _options.CommandAckQueue.Exclusive,
                autoDelete: _options.CommandAckQueue.AutoDelete,
                arguments: queueArgs.Concat(_options.CommandAckQueue.Arguments).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                cancellationToken: cancellationToken);

            await _publishChannel.QueueDeclareAsync(
                queue: _options.StatusUpdateQueue.Name,
                durable: _options.StatusUpdateQueue.Durable,
                exclusive: _options.StatusUpdateQueue.Exclusive,
                autoDelete: _options.StatusUpdateQueue.AutoDelete,
                arguments: queueArgs.Concat(_options.StatusUpdateQueue.Arguments).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                cancellationToken: cancellationToken);

            // Bind queues to exchanges
            await _publishChannel.QueueBindAsync(
                queue: _options.CommandAckQueue.Name,
                exchange: _options.RobotCommandExchange.Name,
                routingKey: _options.CommandAckQueue.RoutingKey,
                cancellationToken: cancellationToken);

            await _publishChannel.QueueBindAsync(
                queue: _options.StatusUpdateQueue.Name,
                exchange: _options.RobotStatusExchange.Name,
                routingKey: _options.StatusUpdateQueue.RoutingKey,
                cancellationToken: cancellationToken);

            _logger.LogDebug("RabbitMQ topology setup completed successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ topology");
            return Result.Error($"Topology setup failed: {ex.Message}");
        }
    }

    private async Task<Result> StartConsumingAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_consumeChannel == null)
                return Result.Error("Consume channel not available");

            // Set up consumers for acknowledgments
            var ackConsumer = new AsyncEventingBasicConsumer(_consumeChannel);
            ackConsumer.ReceivedAsync += OnCommandAcknowledgmentReceived;

            await _consumeChannel.BasicConsumeAsync(
                queue: _options.CommandAckQueue.Name,
                autoAck: true,
                consumer: ackConsumer,
                cancellationToken: cancellationToken);

            // Set up consumers for status updates
            var statusConsumer = new AsyncEventingBasicConsumer(_consumeChannel);
            statusConsumer.ReceivedAsync += OnStatusUpdateReceived;

            await _consumeChannel.BasicConsumeAsync(
                queue: _options.StatusUpdateQueue.Name,
                autoAck: true,
                consumer: statusConsumer,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Started consuming messages from RabbitMQ queues");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start consuming messages");
            return Result.Error($"Failed to start consuming: {ex.Message}");
        }
    }

    private async Task OnCommandAcknowledgmentReceived(object sender, BasicDeliverEventArgs e)
    {
        try
        {
            var messageBody = Encoding.UTF8.GetString(e.Body.ToArray());
            var acknowledgment = JsonSerializer.Deserialize<CommandAcknowledgmentMessage>(messageBody, _jsonOptions);

            if (acknowledgment == null)
            {
                _logger.LogWarning("Received null acknowledgment message");
                return;
            }

            _logger.LogDebug("Received acknowledgment for command {CommandMessageId} from robot {RobotId}",
                acknowledgment.CommandMessageId, acknowledgment.RobotId);

            // Complete pending command if exists
            if (_pendingCommands.TryRemove(acknowledgment.CommandMessageId, out var tcs))
            {
                tcs.SetResult(acknowledgment);
            }

            // Raise event
            CommandAcknowledged?.Invoke(this, acknowledgment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing command acknowledgment message");
        }
    }

    private async Task OnStatusUpdateReceived(object sender, BasicDeliverEventArgs e)
    {
        try
        {
            var messageBody = Encoding.UTF8.GetString(e.Body.ToArray());
            var statusMessage = JsonSerializer.Deserialize<RobotStatusMessage>(messageBody, _jsonOptions);

            if (statusMessage == null)
            {
                _logger.LogWarning("Received null status message");
                return;
            }

            _logger.LogDebug("Received status {Status} from robot {RobotId}",
                statusMessage.Status, statusMessage.RobotId);

            // Complete pending status request if exists
            if (!string.IsNullOrEmpty(statusMessage.CorrelationId) &&
                _pendingStatusRequests.TryRemove(statusMessage.CorrelationId, out var tcs))
            {
                tcs.SetResult(statusMessage);
            }

            // Raise event
            StatusReceived?.Invoke(this, statusMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing status update message");
        }
    }
}