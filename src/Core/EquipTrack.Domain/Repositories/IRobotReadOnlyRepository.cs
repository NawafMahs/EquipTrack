using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository for Robot entity queries.
/// </summary>
public interface IRobotReadOnlyRepository : IReadOnlyRepository<Robot, Guid>
{
    Task<IEnumerable<Robot>> GetByStatusAsync(RobotStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Robot>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<IEnumerable<Robot>> GetRequiringMaintenanceAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Robot>> GetRequiringChargingAsync(CancellationToken cancellationToken = default);
    Task<Robot?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Robot>> GetByTypeAsync(RobotType robotType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Robot>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Robot?> GetWithSensorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Robot?> GetWithLogsAsync(Guid id, CancellationToken cancellationToken = default);
}
