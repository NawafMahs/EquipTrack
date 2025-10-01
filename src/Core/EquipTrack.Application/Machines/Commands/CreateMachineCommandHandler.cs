using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using MediatR;

namespace EquipTrack.Application.Machines.Commands;

/// <summary>
/// Handler for CreateMachineCommand.
/// </summary>
public sealed class CreateMachineCommandHandler : IRequestHandler<CreateMachineCommand, Result<Guid>>
{
    private readonly IMachineWriteOnlyRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMachineCommandHandler(
        IMachineWriteOnlyRepository machineRepository,
        IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateMachineCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var machine = Machine.Create(
                request.Name,
                request.SerialNumber,
                request.Model,
                request.Manufacturer,
                request.Location,
                request.InstallationDate,
                request.MachineTypeRef
            );

            await _machineRepository.AddAsync(machine);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(machine.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Error($"Failed to create machine: {ex.Message}");
        }
    }
}
