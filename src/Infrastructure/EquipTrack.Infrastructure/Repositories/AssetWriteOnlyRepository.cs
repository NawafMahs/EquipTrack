using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for Asset entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class AssetWriteOnlyRepository : IAssetWriteOnlyRepository
{
    private readonly EquipTrackDbContext _context;
    private readonly DbSet<Asset> _dbSet;

    public AssetWriteOnlyRepository(EquipTrackDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Asset>();
    }

    #region Query Methods (for update operations)

    public virtual async Task<Asset?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    #endregion

    #region Synchronous Methods

    public virtual void Add(Asset entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(Asset entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(Asset entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Delete(Asset entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Remove(Guid id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual void AddRange(IEnumerable<Asset> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void RemoveRange(IEnumerable<Asset> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    #endregion

    #region Asynchronous Methods

    public virtual async Task AddAsync(Asset entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task RemoveByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task AddRangeAsync(IEnumerable<Asset> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}