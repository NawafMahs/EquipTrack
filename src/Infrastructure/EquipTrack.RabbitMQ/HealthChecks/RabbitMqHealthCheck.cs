using EquipTrack.Infrastructure.RabbitMQ.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace EquipTrack.Infrastructure.RabbitMQ.HealthChecks;

/// <summary>
/// Health check for RabbitMQ messaging services.
/// </summary>
public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly IRobotMessagingService _messagingService;
    private readonly ILogger<RabbitMqHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqHealthCheck"/> class.
    /// </summary>
    /// <param name="connectionFactory">The RabbitMQ connection factory.</param>
    /// <param name="messagingService">The robot messaging service.</param>
    /// <param name="logger">The logger instance.</param>
    public RabbitMqHealthCheck(
        IRabbitMqConnectionFactory connectionFactory,
        IRobotMessagingService messagingService,
        ILogger<RabbitMqHealthCheck> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = new Dictionary<string, object>();

            // Check connection factory health
            var connectionHealthResult = _connectionFactory.IsHealthy();
            if (!connectionHealthResult.IsSuccess)
            {
                data["connection-factory-error"] = string.Join(", ", connectionHealthResult.Errors);
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ connection factory is unhealthy",
                    data: data);
            }

            var connectionHealthy = connectionHealthResult.Value;
            data["connection-healthy"] = connectionHealthy;

            if (!connectionHealthy)
            {
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ connection is not healthy",
                    data: data);
            }

            // Check messaging service health
            var messagingHealthResult = await _messagingService.IsHealthyAsync();
            if (!messagingHealthResult.IsSuccess)
            {
                data["messaging-service-error"] = string.Join(", ", messagingHealthResult.Errors);
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ messaging service health check failed",
                    data: data);
            }

            var messagingHealthy = messagingHealthResult.Value;
            data["messaging-service-healthy"] = messagingHealthy;

            if (!messagingHealthy)
            {
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ messaging service is not healthy",
                    data: data);
            }

            // Get connection status for additional data
            var connectionStatusResult = await _messagingService.GetConnectionStatusAsync();
            if (connectionStatusResult.IsSuccess)
            {
                foreach (var kvp in connectionStatusResult.Value)
                {
                    data[kvp.Key] = kvp.Value;
                }
            }

            // Test basic connectivity
            var testResult = await _connectionFactory.TestConnectionAsync(cancellationToken);
            if (!testResult.IsSuccess)
            {
                data["connection-test-error"] = string.Join(", ", testResult.Errors);
                return HealthCheckResult.Degraded(
                    "RabbitMQ connection test failed but services are running",
                    data: data);
            }

            data["connection-test"] = "passed";
            data["last-check"] = DateTime.UtcNow;

            return HealthCheckResult.Healthy(
                "RabbitMQ messaging services are healthy",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RabbitMQ health check");
            
            return HealthCheckResult.Unhealthy(
                "RabbitMQ health check failed with exception",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["last-check"] = DateTime.UtcNow
                });
        }
    }
}