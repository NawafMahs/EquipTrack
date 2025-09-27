using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.WorkOrders.Commands;

/// <summary>
/// Command to create a new work order.
/// </summary>
public sealed record CreateWorkOrderCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="CreateWorkOrderCommand"/>.
    /// </summary>
    /// <param name="title">Work order title.</param>
    /// <param name="description">Work order description.</param>
    /// <param name="assetRef">Asset reference ID.</param>
    /// <param name="assignedToRef">Assigned user reference ID.</param>
    /// <param name="priority">Work order priority.</param>
    /// <param name="type">Work order type.</param>
    /// <param name="scheduledDate">Scheduled date for the work order.</param>
    /// <param name="estimatedHours">Estimated hours to complete.</param>
    public CreateWorkOrderCommand(
        string title,
        string? description,
        Guid assetRef,
        Guid? assignedToRef,
        WorkOrderPriority priority,
        WorkOrderType type,
        DateTime? scheduledDate = null,
        decimal? estimatedHours = null)
    {
        Title = title;
        Description = description;
        AssetRef = assetRef;
        AssignedToRef = assignedToRef;
        Priority = priority;
        Type = type;
        ScheduledDate = scheduledDate;
        EstimatedHours = estimatedHours;
    }

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
    /// Scheduled date for the work order.
    /// </summary>
    public DateTime? ScheduledDate { get; init; }

    /// <summary>
    /// Estimated hours to complete.
    /// </summary>
    public decimal? EstimatedHours { get; init; }
}