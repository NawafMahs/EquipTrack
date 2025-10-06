using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Query.Abstractions;

public interface IQueryableRepository<T> where T : BaseEntity
{
    IQueryable<T> Query { get; }
}