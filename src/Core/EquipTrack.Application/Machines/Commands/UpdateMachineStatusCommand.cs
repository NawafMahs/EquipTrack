using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;
using MediatR;

namespace EquipTrack.Application.Machines.Commands;

/// <summary>
/// Command to update machine status.
/// </summary>
public sealed record UpdateMachineStatusCommand(
    Guid MachineId,
    MachineStatus NewStatus
) : IRequest<Result>;
