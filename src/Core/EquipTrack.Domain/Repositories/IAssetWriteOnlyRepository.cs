using EquipTrack.Domain.Common;
using EquipTrack.Domain.Assets.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for Asset entities.
/// </summary>
public interface IAssetWriteOnlyRepository : IWriteOnlyRepository<Asset, Guid>
{
}