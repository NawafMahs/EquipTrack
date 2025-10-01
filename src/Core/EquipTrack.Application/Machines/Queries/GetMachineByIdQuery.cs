using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Entities;
using MediatR;

namespace EquipTrack.Application.Machines.Queries;

/// <summary>
/// Query to get a machine by its ID.
/// </summary>
public sealed record GetMachineByIdQuery(Guid MachineId) : IRequest<Result<Machine>>;
