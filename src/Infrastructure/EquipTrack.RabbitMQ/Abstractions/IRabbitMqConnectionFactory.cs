using EquipTrack.Core.SharedKernel;
using RabbitMQ.Client;

namespace EquipTrack.Infrastructure.RabbitMQ.Abstractions;

/// <summary>
/// Factory for creating and managing RabbitMQ connections.
/// </summary>
public interface IRabbitMqConnectionFactory : IDisposable
{
    /// <summary>
    /// Creates a new RabbitMQ connection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the connection.</returns>
    Task<Result<IConnection>> CreateConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a shared connection for the application.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the shared connection.</returns>
    Task<Result<IConnection>> GetSharedConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new channel from the shared connection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the channel.</returns>
    Task<Result<IChannel>> CreateChannelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests the connection to RabbitMQ.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating connection success.</returns>
    Task<Result> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current connection status.
    /// </summary>
    /// <returns>A result containing connection status information.</returns>
    Result<Dictionary<string, object>> GetConnectionStatus();

    /// <summary>
    /// Checks if the factory is healthy and connections are available.
    /// </summary>
    /// <returns>A result indicating health status.</returns>
    Result<bool> IsHealthy();
}