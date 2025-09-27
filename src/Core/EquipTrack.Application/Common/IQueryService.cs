namespace EquipTrack.Core.Application.Common;

public interface IQueryService<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
}