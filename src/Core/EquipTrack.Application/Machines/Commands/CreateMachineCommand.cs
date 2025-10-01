using EquipTrack.Core.SharedKernel;
using MediatR;

namespace EquipTrack.Application.Machines.Commands;

/// <summary>
/// Command to create a new machine in the system.
/// </summary>
public sealed record CreateMachineCommand(
    string Name,
    string SerialNumber,
    string Model,
    string Manufacturer,
    string Location,
    DateTime InstallationDate,
    string MachineTypeRef
) : IRequest<Result<Guid>>;
