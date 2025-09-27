using System.ComponentModel.DataAnnotations;

namespace EquipTrack.Infrastructure.RabbitMQ.Configuration;

/// <summary>
/// Configuration options for RabbitMQ connection and messaging.
/// </summary>
public sealed class RabbitMqOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// Gets or sets the RabbitMQ host name.
    /// </summary>
    [Required]
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the RabbitMQ port.
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Gets or sets the virtual host.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    [Required]
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    [Required]
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Gets or sets whether to use SSL/TLS.
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the heartbeat interval in seconds.
    /// </summary>
    [Range(0, 3600)]
    public ushort HeartbeatIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the maximum number of connection retry attempts.
    /// </summary>
    [Range(0, 100)]
    public int MaxRetryAttempts { get; set; } = 5;

    /// <summary>
    /// Gets or sets the retry delay in milliseconds.
    /// </summary>
    [Range(100, 60000)]
    public int RetryDelayMilliseconds { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the exchange configuration for robot commands.
    /// </summary>
    public ExchangeOptions RobotCommandExchange { get; set; } = new()
    {
        Name = "robot-commands",
        Type = "topic",
        Durable = true,
        AutoDelete = false
    };

    /// <summary>
    /// Gets or sets the exchange configuration for robot status updates.
    /// </summary>
    public ExchangeOptions RobotStatusExchange { get; set; } = new()
    {
        Name = "robot-status",
        Type = "topic",
        Durable = true,
        AutoDelete = false
    };

    /// <summary>
    /// Gets or sets the queue configuration for command acknowledgments.
    /// </summary>
    public QueueOptions CommandAckQueue { get; set; } = new()
    {
        Name = "command-acknowledgments",
        Durable = true,
        Exclusive = false,
        AutoDelete = false,
        RoutingKey = "command.ack.#"
    };

    /// <summary>
    /// Gets or sets the queue configuration for status updates.
    /// </summary>
    public QueueOptions StatusUpdateQueue { get; set; } = new()
    {
        Name = "status-updates",
        Durable = true,
        Exclusive = false,
        AutoDelete = false,
        RoutingKey = "status.#"
    };

    /// <summary>
    /// Gets or sets whether to enable message persistence.
    /// </summary>
    public bool EnableMessagePersistence { get; set; } = true;

    /// <summary>
    /// Gets or sets the message time-to-live in milliseconds.
    /// </summary>
    [Range(1000, 86400000)] // 1 second to 24 hours
    public int MessageTtlMilliseconds { get; set; } = 300000; // 5 minutes

    /// <summary>
    /// Gets or sets whether to enable dead letter exchange.
    /// </summary>
    public bool EnableDeadLetterExchange { get; set; } = true;

    /// <summary>
    /// Gets or sets the dead letter exchange name.
    /// </summary>
    public string DeadLetterExchangeName { get; set; } = "dead-letter-exchange";
}

/// <summary>
/// Configuration options for RabbitMQ exchanges.
/// </summary>
public sealed class ExchangeOptions
{
    /// <summary>
    /// Gets or sets the exchange name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exchange type (direct, topic, fanout, headers).
    /// </summary>
    [Required]
    public string Type { get; set; } = "topic";

    /// <summary>
    /// Gets or sets whether the exchange is durable.
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the exchange should auto-delete.
    /// </summary>
    public bool AutoDelete { get; set; } = false;

    /// <summary>
    /// Gets or sets additional exchange arguments.
    /// </summary>
    public Dictionary<string, object> Arguments { get; set; } = new();
}

/// <summary>
/// Configuration options for RabbitMQ queues.
/// </summary>
public sealed class QueueOptions
{
    /// <summary>
    /// Gets or sets the queue name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the queue is durable.
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the queue is exclusive.
    /// </summary>
    public bool Exclusive { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the queue should auto-delete.
    /// </summary>
    public bool AutoDelete { get; set; } = false;

    /// <summary>
    /// Gets or sets the routing key pattern for binding.
    /// </summary>
    public string RoutingKey { get; set; } = "#";

    /// <summary>
    /// Gets or sets additional queue arguments.
    /// </summary>
    public Dictionary<string, object> Arguments { get; set; } = new();
}