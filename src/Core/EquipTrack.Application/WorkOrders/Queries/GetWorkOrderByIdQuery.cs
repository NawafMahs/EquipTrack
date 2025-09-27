using MediatR;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;

namespace EquipTrack.Application.WorkOrders.Queries;

/// <summary>
/// Query to get a specific work order by ID.
/// </summary>
public sealed record GetWorkOrderByIdQuery : IRequest<Result<WorkOrderQuery>>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetWorkOrderByIdQuery"/>.
    /// </summary>
    /// <param name="id">The work order ID.</param>
    public GetWorkOrderByIdQuery(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// The work order ID.
    /// </summary>
    public Guid Id { get; init; }
}