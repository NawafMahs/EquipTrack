using AutoMapper;
using Microsoft.Extensions.Logging;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Infrastructure.Services;

/// <summary>
/// Work order service implementation.
/// Note: This service is deprecated. Use CQRS pattern with MediatR commands and queries instead.
/// </summary>
[Obsolete("Use CQRS pattern with MediatR commands and queries instead.")]
public class WorkOrderService : IWorkOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WorkOrderService> _logger;

    public WorkOrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<WorkOrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<WorkOrderQuery>>> GetAllWorkOrdersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with GetWorkOrdersQuery instead.");
            return Result<IEnumerable<WorkOrderQuery>>.Success(new List<WorkOrderQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all work orders");
            return Result<IEnumerable<WorkOrderQuery>>.Error("An error occurred while retrieving work orders");
        }
    }

    public async Task<Result<WorkOrderQuery>> GetWorkOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with GetWorkOrderByIdQuery instead.");
            return Result<WorkOrderQuery>.NotFound($"Work order with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work order {WorkOrderId}", id);
            return Result<WorkOrderQuery>.Error("An error occurred while retrieving the work order");
        }
    }

    public async Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByAssetAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with GetWorkOrdersByAssetQuery instead.");
            return Result<IEnumerable<WorkOrderQuery>>.Success(new List<WorkOrderQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work orders for asset {AssetId}", assetId);
            return Result<IEnumerable<WorkOrderQuery>>.Error("An error occurred while retrieving work orders by asset");
        }
    }

    public async Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with GetWorkOrdersByUserQuery instead.");
            return Result<IEnumerable<WorkOrderQuery>>.Success(new List<WorkOrderQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work orders for user {UserId}", userId);
            return Result<IEnumerable<WorkOrderQuery>>.Error("An error occurred while retrieving work orders by user");
        }
    }

    public async Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByStatusAsync(WorkOrderStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with GetWorkOrdersByStatusQuery instead.");
            return Result<IEnumerable<WorkOrderQuery>>.Success(new List<WorkOrderQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work orders by status {Status}", status);
            return Result<IEnumerable<WorkOrderQuery>>.Error("An error occurred while retrieving work orders by status");
        }
    }

    public async Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByPriorityAsync(WorkOrderPriority priority, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with GetWorkOrdersByPriorityQuery instead.");
            return Result<IEnumerable<WorkOrderQuery>>.Success(new List<WorkOrderQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving work orders by priority {Priority}", priority);
            return Result<IEnumerable<WorkOrderQuery>>.Error("An error occurred while retrieving work orders by priority");
        }
    }

    public async Task<Result<WorkOrderQuery>> CreateWorkOrderAsync(CreateWorkOrderCommand createWorkOrderDto, Guid createdByUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with CreateWorkOrderCommand instead.");
            return Result<WorkOrderQuery>.Error("Use CQRS pattern with CreateWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating work order");
            return Result<WorkOrderQuery>.Error("An error occurred while creating the work order");
        }
    }

    public async Task<Result<WorkOrderQuery>> UpdateWorkOrderAsync(Guid id, UpdateWorkOrderCommand updateWorkOrderDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with UpdateWorkOrderCommand instead.");
            return Result<WorkOrderQuery>.Error("Use CQRS pattern with UpdateWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating work order {WorkOrderId}", id);
            return Result<WorkOrderQuery>.Error("An error occurred while updating the work order");
        }
    }

    public async Task<Result> DeleteWorkOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with DeleteWorkOrderCommand instead.");
            return Result.Error("Use CQRS pattern with DeleteWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting work order {WorkOrderId}", id);
            return Result.Error("An error occurred while deleting the work order");
        }
    }

    public async Task<Result> AssignWorkOrderAsync(Guid workOrderId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with AssignWorkOrderCommand instead.");
            return Result.Error("Use CQRS pattern with AssignWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning work order {WorkOrderId} to user {UserId}", workOrderId, userId);
            return Result.Error("An error occurred while assigning the work order");
        }
    }

    public async Task<Result> UpdateWorkOrderStatusAsync(Guid id, WorkOrderStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with UpdateWorkOrderStatusCommand instead.");
            return Result.Error("Use CQRS pattern with UpdateWorkOrderStatusCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating work order status for work order {WorkOrderId}", id);
            return Result.Error("An error occurred while updating the work order status");
        }
    }

    public async Task<Result> StartWorkOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with StartWorkOrderCommand instead.");
            return Result.Error("Use CQRS pattern with StartWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting work order {WorkOrderId}", id);
            return Result.Error("An error occurred while starting the work order");
        }
    }

    public async Task<Result> CompleteWorkOrderAsync(Guid id, string? completionNotes, decimal actualHours, decimal actualCost, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with CompleteWorkOrderCommand instead.");
            return Result.Error("Use CQRS pattern with CompleteWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing work order {WorkOrderId}", id);
            return Result.Error("An error occurred while completing the work order");
        }
    }

    public async Task<Result> AddSparePartToWorkOrderAsync(Guid workOrderId, Guid sparePartId, int quantity, decimal unitCost, string? notes, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("WorkOrderService is deprecated. Use CQRS pattern with AddSparePartToWorkOrderCommand instead.");
            return Result.Error("Use CQRS pattern with AddSparePartToWorkOrderCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding spare part {SparePartId} to work order {WorkOrderId}", sparePartId, workOrderId);
            return Result.Error("An error occurred while adding spare part to work order");
        }
    }
}