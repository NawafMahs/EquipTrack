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
            // Generate asset tag from name and timestamp
            var assetTag = $"MCH-{DateTime.UtcNow:yyyyMMddHHmmss}";
            
            var machine = Machine.Create(
                name: request.Name,
                description: $"Machine: {request.Name}",
                serialNumber: request.SerialNumber,
                model: request.Model,
                manufacturer: request.Manufacturer,
                assetTag: assetTag,
                location: request.Location,
                purchaseDate: request.InstallationDate, // Use installation date as purchase date if not provided
                purchasePrice: 0m, // Default to 0 if not provided
                criticality: Domain.Enums.AssetCriticality.Medium,
                machineTypeRef: request.MachineTypeRef
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
