using EquipTrack.Domain.Common;

namespace EquipTrack.Query.Abstractions;

public interface IQueryableRepository<T> where T : BaseEntity
{
    IQueryable<T> Query { get; }
}