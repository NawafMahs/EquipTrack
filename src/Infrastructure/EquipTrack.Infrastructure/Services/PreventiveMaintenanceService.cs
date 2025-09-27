using AutoMapper;
using Microsoft.Extensions.Logging;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Services;

/// <summary>
/// Preventive maintenance service implementation.
/// Note: This service is deprecated. Use CQRS pattern with MediatR commands and queries instead.
/// </summary>
[Obsolete("Use CQRS pattern with MediatR commands and queries instead.")]
public class PreventiveMaintenanceService : IPreventiveMaintenanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PreventiveMaintenanceService> _logger;

    public PreventiveMaintenanceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PreventiveMaintenanceService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetAllPreventiveMaintenancesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with GetPreventiveMaintenancesQuery instead.");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Success(new List<PreventiveMaintenanceQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all preventive maintenances");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Error("An error occurred while retrieving preventive maintenances");
        }
    }

    public async Task<Result<PreventiveMaintenanceQuery>> GetPreventiveMaintenanceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with GetPreventiveMaintenanceByIdQuery instead.");
            return Result<PreventiveMaintenanceQuery>.NotFound($"Preventive maintenance with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preventive maintenance {PreventiveMaintenanceId}", id);
            return Result<PreventiveMaintenanceQuery>.Error("An error occurred while retrieving the preventive maintenance");
        }
    }

    public async Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetPreventiveMaintenancesByAssetAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with GetPreventiveMaintenancesByAssetQuery instead.");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Success(new List<PreventiveMaintenanceQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preventive maintenances for asset {AssetId}", assetId);
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Error("An error occurred while retrieving preventive maintenances by asset");
        }
    }

    public async Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetOverduePreventiveMaintenancesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with GetOverduePreventiveMaintenancesQuery instead.");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Success(new List<PreventiveMaintenanceQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue preventive maintenances");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Error("An error occurred while retrieving overdue preventive maintenances");
        }
    }

    public async Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetDueSoonPreventiveMaintenancesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with GetDueSoonPreventiveMaintenancesQuery instead.");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Success(new List<PreventiveMaintenanceQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving due soon preventive maintenances");
            return Result<IEnumerable<PreventiveMaintenanceQuery>>.Error("An error occurred while retrieving due soon preventive maintenances");
        }
    }

    public async Task<Result<PreventiveMaintenanceQuery>> CreatePreventiveMaintenanceAsync(CreatePreventiveMaintenanceCommand createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with CreatePreventiveMaintenanceCommand instead.");
            return Result<PreventiveMaintenanceQuery>.Error("Use CQRS pattern with CreatePreventiveMaintenanceCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating preventive maintenance");
            return Result<PreventiveMaintenanceQuery>.Error("An error occurred while creating the preventive maintenance");
        }
    }

    public async Task<Result<PreventiveMaintenanceQuery>> UpdatePreventiveMaintenanceAsync(Guid id, UpdatePreventiveMaintenanceCommand updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with UpdatePreventiveMaintenanceCommand instead.");
            return Result<PreventiveMaintenanceQuery>.Error("Use CQRS pattern with UpdatePreventiveMaintenanceCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preventive maintenance {PreventiveMaintenanceId}", id);
            return Result<PreventiveMaintenanceQuery>.Error("An error occurred while updating the preventive maintenance");
        }
    }

    public async Task<Result> DeletePreventiveMaintenanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with DeletePreventiveMaintenanceCommand instead.");
            return Result.Error("Use CQRS pattern with DeletePreventiveMaintenanceCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preventive maintenance {PreventiveMaintenanceId}", id);
            return Result.Error("An error occurred while deleting the preventive maintenance");
        }
    }

    public async Task<Result<WorkOrderQuery>> GenerateWorkOrderFromPreventiveMaintenanceAsync(Guid preventiveMaintenanceId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with GenerateWorkOrderFromPreventiveMaintenanceCommand instead.");
            return Result<WorkOrderQuery>.Error("Use CQRS pattern with GenerateWorkOrderFromPreventiveMaintenanceCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating work order from preventive maintenance {PreventiveMaintenanceId}", preventiveMaintenanceId);
            return Result<WorkOrderQuery>.Error("An error occurred while generating work order from preventive maintenance");
        }
    }

    public async Task<Result> MarkAsCompletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("PreventiveMaintenanceService is deprecated. Use CQRS pattern with MarkPreventiveMaintenanceAsCompletedCommand instead.");
            return Result.Error("Use CQRS pattern with MarkPreventiveMaintenanceAsCompletedCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking preventive maintenance as completed {PreventiveMaintenanceId}", id);
            return Result.Error("An error occurred while marking preventive maintenance as completed");
        }
    }
}