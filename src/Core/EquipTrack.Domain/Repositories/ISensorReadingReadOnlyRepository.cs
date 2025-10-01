using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository for SensorReading entity queries.
/// </summary>
public interface ISensorReadingReadOnlyRepository : IReadOnlyRepository<SensorReading, Guid>
{
    Task<IEnumerable<SensorReading>> GetBySensorAsync(Guid sensorRef, CancellationToken cancellationToken = default);
    Task<IEnumerable<SensorReading>> GetBySensorInTimeRangeAsync(Guid sensorRef, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<SensorReading?> GetLatestBySensorAsync(Guid sensorRef, CancellationToken cancellationToken = default);
}
