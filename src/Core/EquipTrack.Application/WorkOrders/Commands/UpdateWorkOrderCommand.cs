using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.WorkOrders.Commands;

/// <summary>
/// Command to update an existing work order.
/// </summary>
public sealed record UpdateWorkOrderCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="UpdateWorkOrderCommand"/>.
    /// </summary>
    /// <param name="id">Work order ID.</param>
    /// <param name="title">Work order title.</param>
    /// <param name="description">Work order description.</param>
    /// <param name="assetRef">Asset reference ID.</param>
    /// <param name="assignedToRef">Assigned user reference ID.</param>
    /// <param name="priority">Work order priority.</param>
    /// <param name="type">Work order type.</param>
    /// <param name="status">Work order status.</param>
    /// <param name="scheduledDate">Scheduled date for the work order.</param>
    /// <param name="estimatedHours">Estimated hours to complete.</param>
    /// <param name="actualHours">Actual hours spent.</param>
    /// <param name="completedDate">Completion date.</param>
    /// <param name="notes">Work order notes.</param>
    public UpdateWorkOrderCommand(
        Guid id,
        string title,
        string? description,
        Guid assetRef,
        Guid? assignedToRef,
        WorkOrderPriority priority,
        WorkOrderType type,
        WorkOrderStatus status,
        DateTime? scheduledDate = null,
        decimal? estimatedHours = null,
        decimal? actualHours = null,
        DateTime? completedDate = null,
        string? notes = null)
    {
        Id = id;
        Title = title;
        Description = description;
        AssetRef = assetRef;
        AssignedToRef = assignedToRef;
        Priority = priority;
        Type = type;
        Status = status;
        ScheduledDate = scheduledDate;
        EstimatedHours = estimatedHours;
        ActualHours = actualHours;
        CompletedDate = completedDate;
        Notes = notes;
    }

    /// <summary>
    /// Work order ID.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Work order title.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Work order description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Asset reference ID.
    /// </summary>
    public Guid AssetRef { get; init; }

    /// <summary>
    /// Assigned user reference ID.
    /// </summary>
    public Guid? AssignedToRef { get; init; }

    /// <summary>
    /// Work order priority.
    /// </summary>
    public WorkOrderPriority Priority { get; init; }

    /// <summary>
    /// Work order type.
    /// </summary>
    public WorkOrderType Type { get; init; }

    /// <summary>
    /// Work order status.
    /// </summary>
    public WorkOrderStatus Status { get; init; }

    /// <summary>
    /// Scheduled date for the work order.
    /// </summary>
    public DateTime? ScheduledDate { get; init; }

    /// <summary>
    /// Estimated hours to complete.
    /// </summary>
    public decimal? EstimatedHours { get; init; }

    /// <summary>
    /// Actual hours spent.
    /// </summary>
    public decimal? ActualHours { get; init; }

    /// <summary>
    /// Completion date.
    /// </summary>
    public DateTime? CompletedDate { get; init; }

    /// <summary>
    /// Work order notes.
    /// </summary>
    public string? Notes { get; init; }
}