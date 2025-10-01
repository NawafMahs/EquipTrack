using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository for Machine entity queries.
/// </summary>
public interface IMachineReadOnlyRepository : IReadOnlyRepository<Machine, Guid>
{
    Task<IEnumerable<Machine>> GetByStatusAsync(MachineStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> GetRequiringMaintenanceAsync(CancellationToken cancellationToken = default);
    Task<Machine?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> GetByMachineTypeAsync(string machineTypeRef, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Machine?> GetWithSensorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Machine?> GetWithLogsAsync(Guid id, CancellationToken cancellationToken = default);
}
