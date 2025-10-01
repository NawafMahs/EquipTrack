using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Repositories;
using MediatR;

namespace EquipTrack.Application.Machines.Commands;

/// <summary>
/// Handler for UpdateMachineStatusCommand.
/// </summary>
public sealed class UpdateMachineStatusCommandHandler : IRequestHandler<UpdateMachineStatusCommand, Result>
{
    private readonly IMachineReadOnlyRepository _machineReadRepository;
    private readonly IMachineWriteOnlyRepository _machineWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMachineStatusCommandHandler(
        IMachineReadOnlyRepository machineReadRepository,
        IMachineWriteOnlyRepository machineWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _machineReadRepository = machineReadRepository;
        _machineWriteRepository = machineWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateMachineStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var machine = await _machineReadRepository.GetByIdAsync(request.MachineId);
            if (machine == null)
                return Result.Error("Machine not found.");

            machine.ChangeStatus(request.NewStatus);

            _machineWriteRepository.Update(machine);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Error(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to update machine status: {ex.Message}");
        }
    }
}
