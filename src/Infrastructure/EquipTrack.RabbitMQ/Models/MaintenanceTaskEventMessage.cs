using System.Text.Json.Serialization;

namespace EquipTrack.Infrastructure.RabbitMQ.Models;

/// <summary>
/// Base class for maintenance task event messages.
/// Used for publishing domain events to RabbitMQ.
/// </summary>
public abstract class MaintenanceTaskEventMessage
{
    /// <summary>
    /// Gets or sets the unique message identifier.
    /// </summary>
    [JsonPropertyName("message-id")]
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the maintenance task identifier.
    /// </summary>
    [JsonPropertyName("task-id")]
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    [JsonPropertyName("event-type")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the event occurred.
    /// </summary>
    [JsonPropertyName("occurred-at")]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the user who triggered the event.
    /// </summary>
    [JsonPropertyName("triggered-by")]
    public string TriggeredBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation identifier for tracking related messages.
    /// </summary>
    [JsonPropertyName("correlation-id")]
    public string? CorrelationId { get; set; }
}

/// <summary>
/// Event message published when a maintenance task is created.
/// </summary>
public sealed class MaintenanceTaskCreatedEvent : MaintenanceTaskEventMessage
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("task-type")]
    public string TaskType { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("asset-ref")]
    public string AssetRef { get; set; } = string.Empty;

    [JsonPropertyName("asset-name")]
    public string AssetName { get; set; } = string.Empty;

    [JsonPropertyName("scheduled-date")]
    public DateTime ScheduledDate { get; set; }

    [JsonPropertyName("estimated-hours")]
    public decimal EstimatedHours { get; set; }

    [JsonPropertyName("estimated-cost")]
    public decimal EstimatedCost { get; set; }

    public MaintenanceTaskCreatedEvent()
    {
        EventType = "MaintenanceTaskCreated";
    }
}

/// <summary>
/// Event message published when a maintenance task is assigned to a technician.
/// </summary>
public sealed class MaintenanceTaskAssignedEvent : MaintenanceTaskEventMessage
{
    [JsonPropertyName("technician-ref")]
    public string TechnicianRef { get; set; } = string.Empty;

    [JsonPropertyName("technician-name")]
    public string TechnicianName { get; set; } = string.Empty;

    [JsonPropertyName("assigned-at")]
    public DateTime AssignedAt { get; set; }

    public MaintenanceTaskAssignedEvent()
    {
        EventType = "MaintenanceTaskAssigned";
    }
}

/// <summary>
/// Event message published when a maintenance task is started.
/// </summary>
public sealed class MaintenanceTaskStartedEvent : MaintenanceTaskEventMessage
{
    [JsonPropertyName("started-at")]
    public DateTime StartedAt { get; set; }

    [JsonPropertyName("technician-ref")]
    public string TechnicianRef { get; set; } = string.Empty;

    public MaintenanceTaskStartedEvent()
    {
        EventType = "MaintenanceTaskStarted";
    }
}

/// <summary>
/// Event message published when a maintenance task is completed.
/// </summary>
public sealed class MaintenanceTaskCompletedEvent : MaintenanceTaskEventMessage
{
    [JsonPropertyName("completed-at")]
    public DateTime CompletedAt { get; set; }

    [JsonPropertyName("actual-hours")]
    public decimal ActualHours { get; set; }

    [JsonPropertyName("actual-cost")]
    public decimal ActualCost { get; set; }

    [JsonPropertyName("hours-variance")]
    public decimal HoursVariance { get; set; }

    [JsonPropertyName("cost-variance")]
    public decimal CostVariance { get; set; }

    [JsonPropertyName("completion-notes")]
    public string CompletionNotes { get; set; } = string.Empty;

    public MaintenanceTaskCompletedEvent()
    {
        EventType = "MaintenanceTaskCompleted";
    }
}

/// <summary>
/// Event message published when a maintenance task becomes overdue.
/// </summary>
public sealed class MaintenanceTaskOverdueEvent : MaintenanceTaskEventMessage
{
    [JsonPropertyName("scheduled-date")]
    public DateTime ScheduledDate { get; set; }

    [JsonPropertyName("days-overdue")]
    public int DaysOverdue { get; set; }

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("asset-name")]
    public string AssetName { get; set; } = string.Empty;

    [JsonPropertyName("assigned-technician")]
    public string? AssignedTechnician { get; set; }

    public MaintenanceTaskOverdueEvent()
    {
        EventType = "MaintenanceTaskOverdue";
    }
}

/// <summary>
/// Event message published when spare parts are consumed in a maintenance task.
/// </summary>
public sealed class SparePartsConsumedEvent : MaintenanceTaskEventMessage
{
    [JsonPropertyName("spare-parts")]
    public List<SparePartConsumption> SpareParts { get; set; } = new();

    [JsonPropertyName("total-cost")]
    public decimal TotalCost { get; set; }

    public SparePartsConsumedEvent()
    {
        EventType = "SparePartsConsumed";
    }
}

/// <summary>
/// Represents a spare part consumption detail.
/// </summary>
public sealed class SparePartConsumption
{
    [JsonPropertyName("spare-part-ref")]
    public string SparePartRef { get; set; } = string.Empty;

    [JsonPropertyName("part-name")]
    public string PartName { get; set; } = string.Empty;

    [JsonPropertyName("part-number")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("quantity-used")]
    public int QuantityUsed { get; set; }

    [JsonPropertyName("unit-cost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("total-cost")]
    public decimal TotalCost { get; set; }
}


