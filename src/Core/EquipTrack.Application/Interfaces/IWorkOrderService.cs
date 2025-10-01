using EquipTrack.Application.DTOs;
using EquipTrack.Application.WorkOrders.Commands;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.Interfaces;

public interface IWorkOrderService
{
    Task<Result<IEnumerable<WorkOrderQuery>>> GetAllWorkOrdersAsync(CancellationToken cancellationToken = default);
    Task<Result<WorkOrderQuery>> GetWorkOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByAssetAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByStatusAsync(WorkOrderStatus status, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByPriorityAsync(WorkOrderPriority priority, CancellationToken cancellationToken = default);
    Task<Result<WorkOrderQuery>> CreateWorkOrderAsync(CreateWorkOrderCommand createWorkOrderDto, Guid createdByUserId, CancellationToken cancellationToken = default);
    Task<Result<WorkOrderQuery>> UpdateWorkOrderAsync(Guid id, UpdateWorkOrderCommand updateWorkOrderDto, CancellationToken cancellationToken = default);
    Task<Result> DeleteWorkOrderAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> AssignWorkOrderAsync(Guid workOrderId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> StartWorkOrderAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> CompleteWorkOrderAsync(Guid id, string? completionNotes, decimal actualHours, decimal actualCost, CancellationToken cancellationToken = default);
    Task<Result> AddSparePartToWorkOrderAsync(Guid workOrderId, Guid sparePartId, int quantity, decimal unitCost, string? notes, CancellationToken cancellationToken = default);
}