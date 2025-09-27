using EquipTrack.Infrastructure.RabbitMQ.Abstractions;
using EquipTrack.Infrastructure.RabbitMQ.Configuration;
using EquipTrack.Infrastructure.RabbitMQ.HealthChecks;
using EquipTrack.Infrastructure.RabbitMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EquipTrack.Infrastructure.RabbitMQ.Extensions;

/// <summary>
/// Extension methods for configuring RabbitMQ services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ messaging services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The configuration section name (defaults to "RabbitMQ").</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = RabbitMqOptions.SectionName)
    {
        // Configure options
        services.Configure<RabbitMqOptions>(options =>
        {
            var section = configuration.GetSection(sectionName);
            options.HostName = section["HostName"] ?? "localhost";
            options.Port = int.Parse(section["Port"] ?? "5672");
            options.UserName = section["UserName"] ?? "guest";
            options.Password = section["Password"] ?? "guest";
            options.VirtualHost = section["VirtualHost"] ?? "/";
            options.ConnectionTimeoutSeconds = int.Parse(section["ConnectionTimeoutSeconds"] ?? "30");
            options.MaxRetryAttempts = int.Parse(section["MaxRetryAttempts"] ?? "3");
            options.RetryDelayMilliseconds = int.Parse(section["RetryDelayMilliseconds"] ?? "1000");
            options.MessageTtlMilliseconds = int.Parse(section["MessageTtlMilliseconds"] ?? "300000");
        });
        
        // Validate options
        services.AddSingleton<IValidateOptions<RabbitMqOptions>, RabbitMqOptionsValidator>();

        // Register core services
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddSingleton<IRobotMessagingService, RobotMessagingService>();

        // Register background service for initialization
        services.AddHostedService<RabbitMqInitializationService>();

        return services;
    }

    /// <summary>
    /// Adds RabbitMQ messaging services with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure RabbitMQ options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        Action<RabbitMqOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        // Validate options
        services.AddSingleton<IValidateOptions<RabbitMqOptions>, RabbitMqOptionsValidator>();

        // Register core services
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddSingleton<IRobotMessagingService, RobotMessagingService>();

        // Register background service for initialization
        services.AddHostedService<RabbitMqInitializationService>();

        return services;
    }

    /// <summary>
    /// Adds health checks for RabbitMQ services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRabbitMqHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: new[] { "rabbitmq", "messaging" });

        return services;
    }
}

/// <summary>
/// Validator for RabbitMQ options.
/// </summary>
internal sealed class RabbitMqOptionsValidator : IValidateOptions<RabbitMqOptions>
{
    public ValidateOptionsResult Validate(string? name, RabbitMqOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.HostName))
            failures.Add("HostName is required");

        if (options.Port <= 0 || options.Port > 65535)
            failures.Add("Port must be between 1 and 65535");

        if (string.IsNullOrWhiteSpace(options.UserName))
            failures.Add("UserName is required");

        if (string.IsNullOrWhiteSpace(options.Password))
            failures.Add("Password is required");

        if (options.ConnectionTimeoutSeconds <= 0)
            failures.Add("ConnectionTimeoutSeconds must be greater than 0");

        if (options.MaxRetryAttempts < 0)
            failures.Add("MaxRetryAttempts must be non-negative");

        if (options.RetryDelayMilliseconds <= 0)
            failures.Add("RetryDelayMilliseconds must be greater than 0");

        if (string.IsNullOrWhiteSpace(options.RobotCommandExchange.Name))
            failures.Add("RobotCommandExchange.Name is required");

        if (string.IsNullOrWhiteSpace(options.RobotStatusExchange.Name))
            failures.Add("RobotStatusExchange.Name is required");

        if (string.IsNullOrWhiteSpace(options.CommandAckQueue.Name))
            failures.Add("CommandAckQueue.Name is required");

        if (string.IsNullOrWhiteSpace(options.StatusUpdateQueue.Name))
            failures.Add("StatusUpdateQueue.Name is required");

        if (options.MessageTtlMilliseconds <= 0)
            failures.Add("MessageTtlMilliseconds must be greater than 0");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

/// <summary>
/// Background service for initializing RabbitMQ messaging service.
/// </summary>
internal sealed class RabbitMqInitializationService : BackgroundService
{
    private readonly IRobotMessagingService _messagingService;
    private readonly ILogger<RabbitMqInitializationService> _logger;

    public RabbitMqInitializationService(
        IRobotMessagingService messagingService,
        ILogger<RabbitMqInitializationService> logger)
    {
        _messagingService = messagingService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Initializing RabbitMQ messaging service...");
            
            var result = await _messagingService.InitializeAsync(stoppingToken);
            if (result.IsSuccess)
            {
                _logger.LogInformation("RabbitMQ messaging service initialized successfully");
            }
            else
            {
                _logger.LogError("Failed to initialize RabbitMQ messaging service: {Errors}",
                    string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RabbitMQ messaging service initialization");
        }
    }
}