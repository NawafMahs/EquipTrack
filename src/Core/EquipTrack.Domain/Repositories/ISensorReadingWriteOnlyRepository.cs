using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository for SensorReading entity commands.
/// </summary>
public interface ISensorReadingWriteOnlyRepository : IWriteOnlyRepository<SensorReading, Guid>
{
}
