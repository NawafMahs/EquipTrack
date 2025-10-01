using EquipTrack.Infrastructure.RabbitMQ.Abstractions;
using EquipTrack.Infrastructure.RabbitMQ.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace EquipTrack.Infrastructure.RabbitMQ.Services;

/// <summary>
/// Implementation of maintenance event publisher using RabbitMQ.
/// Publishes domain events to RabbitMQ exchanges for event-driven architecture.
/// </summary>
public sealed class MaintenanceEventPublisher : IMaintenanceEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<MaintenanceEventPublisher> _logger;
    
    // Exchange and routing key configuration
    private const string MaintenanceExchange = "cmms.maintenance.events";
    private const string MaintenanceTaskCreatedRoutingKey = "maintenance.task.created";
    private const string MaintenanceTaskAssignedRoutingKey = "maintenance.task.assigned";
    private const string MaintenanceTaskStartedRoutingKey = "maintenance.task.started";
    private const string MaintenanceTaskCompletedRoutingKey = "maintenance.task.completed";
    private const string MaintenanceTaskOverdueRoutingKey = "maintenance.task.overdue";
    private const string SparePartsConsumedRoutingKey = "maintenance.spareparts.consumed";

    public MaintenanceEventPublisher(
        IConnectionFactory connectionFactory,
        ILogger<MaintenanceEventPublisher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        try
        {
            // Create connection and channel
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // Declare the exchange (topic exchange for flexible routing)
            _channel.ExchangeDeclare(
                exchange: MaintenanceExchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);
            
            _logger.LogInformation(
                "MaintenanceEventPublisher initialized with exchange: {Exchange}",
                MaintenanceExchange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MaintenanceEventPublisher");
            throw;
        }
    }

    public async Task PublishMaintenanceTaskCreatedAsync(
        MaintenanceTaskCreatedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            eventMessage,
            MaintenanceTaskCreatedRoutingKey,
            cancellationToken);
    }

    public async Task PublishMaintenanceTaskAssignedAsync(
        MaintenanceTaskAssignedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            eventMessage,
            MaintenanceTaskAssignedRoutingKey,
            cancellationToken);
    }

    public async Task PublishMaintenanceTaskStartedAsync(
        MaintenanceTaskStartedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            eventMessage,
            MaintenanceTaskStartedRoutingKey,
            cancellationToken);
    }

    public async Task PublishMaintenanceTaskCompletedAsync(
        MaintenanceTaskCompletedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            eventMessage,
            MaintenanceTaskCompletedRoutingKey,
            cancellationToken);
    }

    public async Task PublishMaintenanceTaskOverdueAsync(
        MaintenanceTaskOverdueEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            eventMessage,
            MaintenanceTaskOverdueRoutingKey,
            cancellationToken);
    }

    public async Task PublishSparePartsConsumedAsync(
        SparePartsConsumedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            eventMessage,
            SparePartsConsumedRoutingKey,
            cancellationToken);
    }

    /// <summary>
    /// Generic method to publish any event to RabbitMQ.
    /// </summary>
    private Task PublishEventAsync<T>(
        T eventMessage,
        string routingKey,
        CancellationToken cancellationToken) where T : MaintenanceTaskEventMessage
    {
        try
        {
            // Serialize the event message
            var json = JsonSerializer.Serialize(eventMessage, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
            
            var body = Encoding.UTF8.GetBytes(json);
            
            // Set message properties
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = eventMessage.MessageId;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Type = eventMessage.EventType;
            
            if (!string.IsNullOrEmpty(eventMessage.CorrelationId))
            {
                properties.CorrelationId = eventMessage.CorrelationId;
            }
            
            // Publish the message
            _channel.BasicPublish(
                exchange: MaintenanceExchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
            
            _logger.LogInformation(
                "Published event {EventType} with ID {MessageId} to routing key {RoutingKey}",
                eventMessage.EventType,
                eventMessage.MessageId,
                routingKey);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish event {EventType} with ID {MessageId}",
                eventMessage.EventType,
                eventMessage.MessageId);
            
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            
            _logger.LogInformation("MaintenanceEventPublisher disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing MaintenanceEventPublisher");
        }
    }
}


