using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for WorkOrder entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class WorkOrderWriteOnlyRepository : IWorkOrderWriteOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<WorkOrder> _dbSet;

    public WorkOrderWriteOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkOrder>();
    }

    #region Query Methods (for update operations)

    public virtual async Task<WorkOrder?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    #endregion

    #region Synchronous Methods

    public virtual void Add(WorkOrder entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(WorkOrder entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(WorkOrder entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Delete(WorkOrder entity)
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

    public virtual void AddRange(IEnumerable<WorkOrder> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void RemoveRange(IEnumerable<WorkOrder> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    #endregion

    #region Asynchronous Methods

    public virtual async Task AddAsync(WorkOrder entity)
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

    public virtual async Task AddRangeAsync(IEnumerable<WorkOrder> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}