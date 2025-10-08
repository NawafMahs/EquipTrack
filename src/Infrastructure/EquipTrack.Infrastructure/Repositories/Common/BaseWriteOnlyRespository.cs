using EquipTrack.Core.SharedKernel;
using EquipTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipTrack.Infrastructure.Repositories.Common;

internal abstract class BaseWriteOnlyRespository<TEntity, TKey>(ApplicationDbContext appDbContext)
    : IWriteOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{


    private static readonly Func<ApplicationDbContext, TKey, Task<TEntity>> GetByIdCompiledAsync =
        EF.CompileAsyncQuery((ApplicationDbContext appDbContext, TKey id) =>
        appDbContext
        .Set<TEntity>()
        .AsNoTrackingWithIdentityResolution()
        .FirstOrDefault(entity => entity.Id.Equals(id))
        );

    public void Add(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetByIdAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public void Remove(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
