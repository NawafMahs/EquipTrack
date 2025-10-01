using EquipTrack.Infrastructure.RabbitMQ.Models;

namespace EquipTrack.Infrastructure.RabbitMQ.Abstractions;

/// <summary>
/// Service for publishing maintenance task events to RabbitMQ.
/// Enables event-driven architecture and system integration.
/// </summary>
public interface IMaintenanceEventPublisher
{
    /// <summary>
    /// Publishes a maintenance task created event.
    /// </summary>
    Task PublishMaintenanceTaskCreatedAsync(
        MaintenanceTaskCreatedEvent eventMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a maintenance task assigned event.
    /// </summary>
    Task PublishMaintenanceTaskAssignedAsync(
        MaintenanceTaskAssignedEvent eventMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a maintenance task started event.
    /// </summary>
    Task PublishMaintenanceTaskStartedAsync(
        MaintenanceTaskStartedEvent eventMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a maintenance task completed event.
    /// </summary>
    Task PublishMaintenanceTaskCompletedAsync(
        MaintenanceTaskCompletedEvent eventMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a maintenance task overdue event.
    /// </summary>
    Task PublishMaintenanceTaskOverdueAsync(
        MaintenanceTaskOverdueEvent eventMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a spare parts consumed event.
    /// </summary>
    Task PublishSparePartsConsumedAsync(
        SparePartsConsumedEvent eventMessage,
        CancellationToken cancellationToken = default);
}


