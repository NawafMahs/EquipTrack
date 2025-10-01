using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository for Robot entity commands.
/// </summary>
public interface IRobotWriteOnlyRepository : IWriteOnlyRepository<Robot, Guid>
{
}
