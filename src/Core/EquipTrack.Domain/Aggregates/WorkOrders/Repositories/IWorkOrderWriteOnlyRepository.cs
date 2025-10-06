using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for WorkOrder entities.
/// </summary>
public interface IWorkOrderWriteOnlyRepository : IWriteOnlyRepository<WorkOrder, Guid>
{
}