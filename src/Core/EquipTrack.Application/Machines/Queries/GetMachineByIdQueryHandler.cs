using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using MediatR;

namespace EquipTrack.Application.Machines.Queries;

/// <summary>
/// Handler for GetMachineByIdQuery.
/// </summary>
public sealed class GetMachineByIdQueryHandler : IRequestHandler<GetMachineByIdQuery, Result<Machine>>
{
    private readonly IMachineReadOnlyRepository _machineRepository;

    public GetMachineByIdQueryHandler(IMachineReadOnlyRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    public async Task<Result<Machine>> Handle(GetMachineByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
            
            if (machine == null)
                return Result<Machine>.Failure("Machine not found.");

            return Result<Machine>.Success(machine);
        }
        catch (Exception ex)
        {
            return Result<Machine>.Failure($"Failed to retrieve machine: {ex.Message}");
        }
    }
}
