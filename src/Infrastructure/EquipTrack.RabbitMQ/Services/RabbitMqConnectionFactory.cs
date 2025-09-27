using EquipTrack.Core.SharedKernel;
using EquipTrack.Infrastructure.RabbitMQ.Abstractions;
using EquipTrack.Infrastructure.RabbitMQ.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;

namespace EquipTrack.Infrastructure.RabbitMQ.Services;

/// <summary>
/// Factory for creating and managing RabbitMQ connections with connection pooling and retry logic.
/// </summary>
public sealed class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;
    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, IConnection> _connections = new();
    
    private IConnection? _sharedConnection;
    private bool _disposed;
    private DateTime _lastConnectionAttempt = DateTime.MinValue;
    private int _consecutiveFailures;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConnectionFactory"/> class.
    /// </summary>
    /// <param name="options">The RabbitMQ configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public RabbitMqConnectionFactory(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConnectionFactory> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result<IConnection>> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionFactory = CreateConnectionFactory();
            var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            
            _consecutiveFailures = 0;
            _logger.LogDebug("Successfully created new RabbitMQ connection to {HostName}:{Port}", 
                _options.HostName, _options.Port);
            
            return Result<IConnection>.Success(connection);
        }
        catch (BrokerUnreachableException ex)
        {
            _consecutiveFailures++;
            _lastConnectionAttempt = DateTime.UtcNow;
            _logger.LogError(ex, "RabbitMQ broker is unreachable at {HostName}:{Port}. Consecutive failures: {Failures}",
                _options.HostName, _options.Port, _consecutiveFailures);
            
            return Result<IConnection>.Error($"RabbitMQ broker is unreachable: {ex.Message}");
        }
        catch (AuthenticationFailureException ex)
        {
            _logger.LogError(ex, "Authentication failed for RabbitMQ connection with user {UserName}",
                _options.UserName);
            
            return Result<IConnection>.Error($"Authentication failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            _lastConnectionAttempt = DateTime.UtcNow;
            _logger.LogError(ex, "Failed to create RabbitMQ connection. Consecutive failures: {Failures}",
                _consecutiveFailures);
            
            return Result<IConnection>.Error($"Failed to create connection: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IConnection>> GetSharedConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return Result<IConnection>.Error("Connection factory has been disposed");

        await _connectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Check if we have a healthy shared connection
            if (_sharedConnection?.IsOpen == true)
            {
                return Result<IConnection>.Success(_sharedConnection);
            }

            // Clean up the old connection if it exists
            if (_sharedConnection != null)
            {
                try
                {
                    await _sharedConnection.CloseAsync();
                    _sharedConnection.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disposing old shared connection");
                }
                finally
                {
                    _sharedConnection = null;
                }
            }

            // Create a new shared connection with retry logic
            var connectionResult = await CreateConnectionWithRetryAsync(cancellationToken);
            if (!connectionResult.IsSuccess)
            {
                return connectionResult;
            }

            _sharedConnection = connectionResult.Value;
            
            // Set up connection recovery event handlers
            // TODO: Re-enable event handlers once RabbitMQ.Client event args are resolved
            // _sharedConnection.ConnectionShutdownAsync += OnConnectionShutdown;
            // _sharedConnection.ConnectionBlockedAsync += OnConnectionBlocked;
            // _sharedConnection.ConnectionUnblockedAsync += OnConnectionUnblocked;

            _logger.LogInformation("Established shared RabbitMQ connection to {HostName}:{Port}",
                _options.HostName, _options.Port);

            return Result<IConnection>.Success(_sharedConnection);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<Result<IChannel>> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        var connectionResult = await GetSharedConnectionAsync(cancellationToken);
        if (!connectionResult.IsSuccess)
        {
            return Result<IChannel>.Error(connectionResult.Errors.ToArray());
        }

        try
        {
            var channel = await connectionResult.Value.CreateChannelAsync(cancellationToken: cancellationToken);
            
            _logger.LogDebug("Successfully created new RabbitMQ channel");
            return Result<IChannel>.Success(channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ channel");
            return Result<IChannel>.Error($"Failed to create channel: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionResult = await CreateConnectionAsync(cancellationToken);
            if (!connectionResult.IsSuccess)
            {
                return Result.Error(connectionResult.Errors.ToArray());
            }

            using var connection = connectionResult.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            
            // Test basic operations
            await channel.ExchangeDeclareAsync(
                exchange: "test-connection-exchange",
                type: ExchangeType.Direct,
                durable: false,
                autoDelete: true,
                cancellationToken: cancellationToken);

            await channel.ExchangeDeleteAsync(
                exchange: "test-connection-exchange",
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ connection test successful");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ connection test failed");
            return Result.Error($"Connection test failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Result<Dictionary<string, object>> GetConnectionStatus()
    {
        var status = new Dictionary<string, object>
        {
            ["is-healthy"] = IsHealthy().Value,
            ["host-name"] = _options.HostName,
            ["port"] = _options.Port,
            ["virtual-host"] = _options.VirtualHost,
            ["user-name"] = _options.UserName,
            ["use-ssl"] = _options.UseSsl,
            ["shared-connection-open"] = _sharedConnection?.IsOpen ?? false,
            ["consecutive-failures"] = _consecutiveFailures,
            ["last-connection-attempt"] = _lastConnectionAttempt,
            ["connection-count"] = _connections.Count
        };

        return Result<Dictionary<string, object>>.Success(status);
    }

    /// <inheritdoc />
    public Result<bool> IsHealthy()
    {
        if (_disposed)
            return Result<bool>.Success(false);

        var isHealthy = _sharedConnection?.IsOpen == true && _consecutiveFailures < _options.MaxRetryAttempts;
        return Result<bool>.Success(isHealthy);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            // Close and dispose shared connection
            if (_sharedConnection != null)
            {
                // TODO: Re-enable event handler cleanup once RabbitMQ.Client event args are resolved
                // _sharedConnection.ConnectionShutdownAsync -= OnConnectionShutdown;
                // _sharedConnection.ConnectionBlockedAsync -= OnConnectionBlocked;
                // _sharedConnection.ConnectionUnblockedAsync -= OnConnectionUnblocked;

                if (_sharedConnection.IsOpen)
                {
                    _sharedConnection.CloseAsync().GetAwaiter().GetResult();
                }
                _sharedConnection.Dispose();
            }

            // Close and dispose all tracked connections
            foreach (var connection in _connections.Values)
            {
                try
                {
                    if (connection.IsOpen)
                    {
                        connection.CloseAsync().GetAwaiter().GetResult();
                    }
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disposing connection during cleanup");
                }
            }

            _connections.Clear();
            _connectionSemaphore.Dispose();

            _logger.LogInformation("RabbitMQ connection factory disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RabbitMQ connection factory disposal");
        }
    }

    private ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.UserName,
            Password = _options.Password,
            Ssl = _options.UseSsl ? new SslOption { Enabled = true } : new SslOption { Enabled = false },
            RequestedHeartbeat = TimeSpan.FromSeconds(_options.HeartbeatIntervalSeconds),
            RequestedConnectionTimeout = TimeSpan.FromSeconds(_options.ConnectionTimeoutSeconds),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            TopologyRecoveryEnabled = true
        };
    }

    private async Task<Result<IConnection>> CreateConnectionWithRetryAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        Exception? lastException = null;

        while (attempt < _options.MaxRetryAttempts)
        {
            try
            {
                var connectionResult = await CreateConnectionAsync(cancellationToken);
                if (connectionResult.IsSuccess)
                {
                    return connectionResult;
                }

                lastException = new Exception(string.Join(", ", connectionResult.Errors));
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            attempt++;
            if (attempt < _options.MaxRetryAttempts)
            {
                var delay = TimeSpan.FromMilliseconds(_options.RetryDelayMilliseconds * attempt);
                _logger.LogWarning("Connection attempt {Attempt} failed, retrying in {Delay}ms. Error: {Error}",
                    attempt, delay.TotalMilliseconds, lastException?.Message);

                await Task.Delay(delay, cancellationToken);
            }
        }

        _logger.LogError(lastException, "Failed to establish RabbitMQ connection after {Attempts} attempts",
            _options.MaxRetryAttempts);

        return Result<IConnection>.Error($"Failed to connect after {_options.MaxRetryAttempts} attempts: {lastException?.Message}");
    }

    // TODO: Re-implement event handlers once RabbitMQ.Client event args are resolved
    // private Task OnConnectionShutdown(object sender, global::RabbitMQ.Client.Events.ConnectionShutdownEventArgs e)
    // {
    //     _logger.LogWarning("RabbitMQ connection shutdown: {Reason}", e.ReplyText);
    //     return Task.CompletedTask;
    // }

    // private Task OnConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    // {
    //     _logger.LogWarning("RabbitMQ connection blocked: {Reason}", e.Reason);
    //     return Task.CompletedTask;
    // }

    // private Task OnConnectionUnblocked(object sender, EventArgs e)
    // {
    //     _logger.LogInformation("RabbitMQ connection unblocked");
    //     return Task.CompletedTask;
    // }
}