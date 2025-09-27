using MediatR;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.WorkOrders.Commands;

/// <summary>
/// Command to delete a work order.
/// </summary>
public sealed record DeleteWorkOrderCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="DeleteWorkOrderCommand"/>.
    /// </summary>
    /// <param name="id">Work order ID to delete.</param>
    public DeleteWorkOrderCommand(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Work order ID to delete.
    /// </summary>
    public Guid Id { get; init; }
}