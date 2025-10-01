using EquipTrack.Core.SharedKernel;
using EquipTrack.Infrastructure.RabbitMQ.Models;
using EquipTrack.RabbitMQ.Models;

namespace EquipTrack.Infrastructure.RabbitMQ.Abstractions;

/// <summary>
/// Service for handling robot communication via RabbitMQ messaging.
/// </summary>
public interface IRobotMessagingService : IDisposable
{
    /// <summary>
    /// Event raised when a status update is received from a robot.
    /// </summary>
    event EventHandler<RobotStatusMessage>? StatusReceived;

    /// <summary>
    /// Event raised when a command acknowledgment is received from a robot.
    /// </summary>
    event EventHandler<CommandAcknowledgmentMessage>? CommandAcknowledged;

    /// <summary>
    /// Initializes the messaging service and establishes connections.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command to a specific robot.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> SendCommandAsync(RobotCommandMessage command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command to a robot and waits for acknowledgment.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="timeoutSeconds">The timeout in seconds to wait for acknowledgment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the acknowledgment message.</returns>
    Task<Result<CommandAcknowledgmentMessage>> SendCommandAndWaitForAckAsync(
        RobotCommandMessage command,
        int timeoutSeconds = 30,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a start production command to a robot.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="recipeId">The recipe identifier.</param>
    /// <param name="batchNumber">The batch number.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> StartProductionAsync(
        string robotId,
        string recipeId,
        string batchNumber,
        string initiatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a stop production command to a robot.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="reason">The reason for stopping.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> StopProductionAsync(
        string robotId,
        string reason,
        string initiatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an emergency stop command to a robot.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> EmergencyStopAsync(
        string robotId,
        string initiatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests status from a specific robot.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> RequestStatusAsync(
        string robotId,
        string initiatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests status from a robot and waits for the response.
    /// </summary>
    /// <param name="robotId">The robot identifier.</param>
    /// <param name="initiatedBy">Who initiated the request.</param>
    /// <param name="timeoutSeconds">The timeout in seconds to wait for response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the status message.</returns>
    Task<Result<RobotStatusMessage>> RequestStatusAndWaitAsync(
        string robotId,
        string initiatedBy,
        int timeoutSeconds = 30,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a status message (typically used for testing or simulation).
    /// </summary>
    /// <param name="statusMessage">The status message to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> PublishStatusAsync(RobotStatusMessage statusMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a command acknowledgment (typically used for testing or simulation).
    /// </summary>
    /// <param name="acknowledgment">The acknowledgment message to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> PublishAcknowledgmentAsync(CommandAcknowledgmentMessage acknowledgment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the messaging service is connected and healthy.
    /// </summary>
    /// <returns>A result indicating the health status.</returns>
    Task<Result<bool>> IsHealthyAsync();

    /// <summary>
    /// Gets the connection status information.
    /// </summary>
    /// <returns>A result containing connection status information.</returns>
    Task<Result<Dictionary<string, object>>> GetConnectionStatusAsync();
}