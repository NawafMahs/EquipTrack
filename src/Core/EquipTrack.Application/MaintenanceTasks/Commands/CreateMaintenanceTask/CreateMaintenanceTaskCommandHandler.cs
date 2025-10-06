using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EquipTrack.Application.MaintenanceTasks.Commands.CreateMaintenanceTask;

/// <summary>
/// Handler for creating a new maintenance task.
/// Implements CQRS command handler pattern with proper validation and error handling.
/// </summary>
public sealed class CreateMaintenanceTaskCommandHandler 
    : IRequestHandler<CreateMaintenanceTaskCommand, Result<Guid>>
{
    private readonly IMaintenanceTaskWriteOnlyRepository _repository;
    private readonly IAssetReadOnlyRepository _assetRepository;
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateMaintenanceTaskCommandHandler> _logger;

    public CreateMaintenanceTaskCommandHandler(
        IMaintenanceTaskWriteOnlyRepository repository,
        IAssetReadOnlyRepository assetRepository,
        IUserReadOnlyRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateMaintenanceTaskCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _assetRepository = assetRepository ?? throw new ArgumentNullException(nameof(assetRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Guid>> Handle(
        CreateMaintenanceTaskCommand command, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Creating maintenance task for Asset: {AssetRef}, Type: {Type}, Priority: {Priority}",
                command.AssetRef, command.Type, command.Priority);

            // Verify asset exists
            var assetExists = await _assetRepository.AnyAsync(command.AssetRef);
            if (!assetExists)
            {
                _logger.LogWarning("Asset with ID {AssetRef} not found", command.AssetRef);
                return Result<Guid>.Error($"Asset with ID {command.AssetRef} not found");
            }

            // Verify creator exists
            var creatorExists = await _userRepository.AnyAsync(command.CreatedByRef);
            if (!creatorExists)
            {
                _logger.LogWarning("User with ID {CreatedByRef} not found", command.CreatedByRef);
                return Result<Guid>.Error($"User with ID {command.CreatedByRef} not found");
            }

            // If technician is assigned, verify they exist
            if (command.AssignedTechnicianRef.HasValue)
            {
                var technicianExists = await _userRepository.AnyAsync(command.AssignedTechnicianRef.Value);
                    
                if (!technicianExists)
                {
                    _logger.LogWarning(
                        "Technician with ID {TechnicianRef} not found", 
                        command.AssignedTechnicianRef.Value);
                        
                    return Result<Guid>.Error(
                        $"Technician with ID {command.AssignedTechnicianRef.Value} not found");
                }
            }

            // Create maintenance task entity using factory method
            var maintenanceTask = MaintenanceTask.Create(
                command.Title,
                command.Description,
                command.Type,
                command.Priority,
                command.AssetRef,
                command.CreatedByRef,
                command.ScheduledDate,
                command.EstimatedHours,
                command.EstimatedCost);

            // Assign technician if provided
            if (command.AssignedTechnicianRef.HasValue)
            {
                var assignResult = maintenanceTask.AssignTechnician(command.AssignedTechnicianRef.Value);
                if (!assignResult.IsSuccess)
                {
                    var errorMessage = string.Join(", ", assignResult.Errors);
                    _logger.LogWarning(
                        "Failed to assign technician: {Error}", 
                        errorMessage);
                        
                    return Result<Guid>.Error(errorMessage);
                }
            }

            // Add to repository
            await _repository.AddAsync(maintenanceTask);

            // Save changes
             await _unitOfWork.SaveChangesAsync(cancellationToken);
            

            _logger.LogInformation(
                "Successfully created maintenance task with ID: {TaskId}", 
                maintenanceTask.Id);

            return Result<Guid>.Success(maintenanceTask.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating maintenance task");
            return Result<Guid>.Error($"Error creating maintenance task: {ex.Message}");
        }
    }
}


