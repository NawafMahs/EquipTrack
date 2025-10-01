using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository for Machine entity commands.
/// </summary>
public interface IMachineWriteOnlyRepository : IWriteOnlyRepository<Machine, Guid>
{
}
