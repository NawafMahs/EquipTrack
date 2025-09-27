using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.WorkOrders.Commands;

/// <summary>
/// Command to update a work order's status.
/// </summary>
public sealed record UpdateWorkOrderStatusCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="UpdateWorkOrderStatusCommand"/>.
    /// </summary>
    /// <param name="id">Work order ID.</param>
    /// <param name="status">New work order status.</param>
    /// <param name="notes">Optional notes for the status change.</param>
    /// <param name="completedDate">Completion date if status is completed.</param>
    /// <param name="actualHours">Actual hours if completing the work order.</param>
    public UpdateWorkOrderStatusCommand(
        Guid id, 
        WorkOrderStatus status, 
        string? notes = null,
        DateTime? completedDate = null,
        decimal? actualHours = null)
    {
        Id = id;
        Status = status;
        Notes = notes;
        CompletedDate = completedDate;
        ActualHours = actualHours;
    }

    /// <summary>
    /// Work order ID.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// New work order status.
    /// </summary>
    public WorkOrderStatus Status { get; init; }

    /// <summary>
    /// Optional notes for the status change.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Completion date if status is completed.
    /// </summary>
    public DateTime? CompletedDate { get; init; }

    /// <summary>
    /// Actual hours if completing the work order.
    /// </summary>
    public decimal? ActualHours { get; init; }
}