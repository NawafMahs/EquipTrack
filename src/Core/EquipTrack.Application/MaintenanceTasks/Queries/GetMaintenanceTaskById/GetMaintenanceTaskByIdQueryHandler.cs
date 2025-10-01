using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EquipTrack.Application.MaintenanceTasks.Queries.GetMaintenanceTaskById;

/// <summary>
/// Handler for GetMaintenanceTaskByIdQuery.
/// Uses projections to transform domain entities into read-optimized DTOs.
/// </summary>
public sealed class GetMaintenanceTaskByIdQueryHandler 
    : IRequestHandler<GetMaintenanceTaskByIdQuery, Result<MaintenanceTaskProjection>>
{
    private readonly IMaintenanceTaskReadOnlyRepository _repository;
    private readonly ILogger<GetMaintenanceTaskByIdQueryHandler> _logger;

    public GetMaintenanceTaskByIdQueryHandler(
        IMaintenanceTaskReadOnlyRepository repository,
        ILogger<GetMaintenanceTaskByIdQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<MaintenanceTaskProjection>> Handle(
        GetMaintenanceTaskByIdQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting maintenance task with ID: {TaskId}", query.Id);

            // Use projection directly in the query for optimal performance
            var projection = await _repository
                .GetQueryable()
                .Where(mt => mt.Id == query.Id)
                .Select(mt => new MaintenanceTaskProjection
                {
                    Id = mt.Id,
                    Title = mt.Title,
                    Description = mt.Description,
                    Type = mt.Type.ToString(),
                    Priority = mt.Priority.ToString(),
                    Status = mt.Status.ToString(),
                    
                    AssetRef = mt.AssetRef,
                    AssetName = mt.Asset.Name,
                    AssetSerialNumber = mt.Asset.SerialNumber,
                    
                    AssignedTechnicianRef = mt.AssignedTechnicianRef,
                    AssignedTechnicianName = mt.AssignedTechnician != null 
                        ? $"{mt.AssignedTechnician.FirstName} {mt.AssignedTechnician.LastName}"
                        : null,
                    
                    CreatedByRef = mt.CreatedByRef,
                    CreatedByName = $"{mt.CreatedBy.FirstName} {mt.CreatedBy.LastName}",
                    
                    ScheduledDate = mt.ScheduledDate,
                    StartedDate = mt.StartedDate,
                    CompletedDate = mt.CompletedDate,
                    
                    EstimatedHours = mt.EstimatedHours,
                    ActualHours = mt.ActualHours,
                    EstimatedCost = mt.EstimatedCost,
                    ActualCost = mt.ActualCost,
                    HoursVariance = mt.ActualHours - mt.EstimatedHours,
                    CostVariance = mt.ActualCost - mt.EstimatedCost,
                    
                    CompletionNotes = mt.CompletionNotes,
                    IsOverdue = mt.Status != MaintenanceTaskStatus.Completed 
                        && mt.Status != MaintenanceTaskStatus.Cancelled 
                        && mt.ScheduledDate < DateTime.UtcNow,
                    
                    SpareParts = mt.SpareParts.Select(sp => new SparePartUsageProjection
                    {
                        SparePartRef = sp.SparePartRef,
                        SparePartName = sp.SparePart.Name,
                        PartNumber = sp.SparePart.PartNumber,
                        QuantityUsed = sp.QuantityUsed,
                        UnitCost = sp.UnitCost,
                        TotalCost = sp.TotalCost
                    }).ToList(),
                    
                    CreatedAt = mt.CreatedAt,
                    UpdatedAt = mt.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (projection == null)
            {
                _logger.LogWarning("Maintenance task with ID {TaskId} not found", query.Id);
                return Result<MaintenanceTaskProjection>.Failure(
                    $"Maintenance task with ID {query.Id} not found");
            }

            _logger.LogInformation("Successfully retrieved maintenance task with ID: {TaskId}", query.Id);
            
            return Result<MaintenanceTaskProjection>.Success(projection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving maintenance task with ID: {TaskId}", query.Id);
            return Result<MaintenanceTaskProjection>.Failure(
                $"Error retrieving maintenance task: {ex.Message}");
        }
    }
}

// Note: We need to add the using statement for MaintenanceTaskStatus
using EquipTrack.Domain.Enums;


