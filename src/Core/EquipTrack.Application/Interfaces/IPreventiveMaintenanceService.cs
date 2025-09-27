using EquipTrack.Application.DTOs;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Interfaces;

public interface IPreventiveMaintenanceService
{
    Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetAllPreventiveMaintenancesAsync(CancellationToken cancellationToken = default);
    Task<Result<PreventiveMaintenanceQuery>> GetPreventiveMaintenanceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetPreventiveMaintenancesByAssetAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetOverduePreventiveMaintenancesAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PreventiveMaintenanceQuery>>> GetDueSoonPreventiveMaintenancesAsync(CancellationToken cancellationToken = default);
    Task<Result<PreventiveMaintenanceQuery>> CreatePreventiveMaintenanceAsync(CreatePreventiveMaintenanceCommand createDto, CancellationToken cancellationToken = default);
    Task<Result<PreventiveMaintenanceQuery>> UpdatePreventiveMaintenanceAsync(Guid id, UpdatePreventiveMaintenanceCommand updateDto, CancellationToken cancellationToken = default);
    Task<Result> DeletePreventiveMaintenanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<WorkOrderQuery>> GenerateWorkOrderFromPreventiveMaintenanceAsync(Guid preventiveMaintenanceId, CancellationToken cancellationToken = default);
    Task<Result> MarkAsCompletedAsync(Guid id, CancellationToken cancellationToken = default);
}