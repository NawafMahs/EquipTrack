using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for PreventiveMaintenance entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class PreventiveMaintenanceWriteOnlyRepository : IPreventiveMaintenanceWriteOnlyRepository
{
    private readonly EquipTrackDbContext _context;
    private readonly DbSet<PreventiveMaintenance> _dbSet;

    public PreventiveMaintenanceWriteOnlyRepository(EquipTrackDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<PreventiveMaintenance>();
    }

    #region Base Repository Methods

    public virtual async Task<PreventiveMaintenance?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual void Add(PreventiveMaintenance entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(PreventiveMaintenance entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(PreventiveMaintenance entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Delete(PreventiveMaintenance entity)
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

    public virtual void AddRange(IEnumerable<PreventiveMaintenance> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void RemoveRange(IEnumerable<PreventiveMaintenance> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task AddAsync(PreventiveMaintenance entity)
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

    public virtual async Task AddRangeAsync(IEnumerable<PreventiveMaintenance> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    #region PreventiveMaintenance-Specific Methods

    public async Task UpdateNextDueDateAsync(Guid preventiveMaintenanceId, DateTime nextDueDate, string updatedBy)
    {
        var preventiveMaintenance = await _dbSet.FindAsync(preventiveMaintenanceId);
        if (preventiveMaintenance != null)
        {
            preventiveMaintenance.NextDueDate = nextDueDate;
            preventiveMaintenance.SetUpdated(updatedBy);
            _dbSet.Update(preventiveMaintenance);
        }
    }

    public async Task MarkAsCompletedAsync(Guid preventiveMaintenanceId, DateTime completedDate, DateTime nextDueDate, string updatedBy)
    {
        var preventiveMaintenance = await _dbSet.FindAsync(preventiveMaintenanceId);
        if (preventiveMaintenance != null)
        {
            preventiveMaintenance.LastCompletedDate = completedDate;
            preventiveMaintenance.NextDueDate = nextDueDate;
            preventiveMaintenance.SetUpdated(updatedBy);
            _dbSet.Update(preventiveMaintenance);
        }
    }

    public async Task SetActiveStatusAsync(Guid preventiveMaintenanceId, bool isActive, string updatedBy)
    {
        var preventiveMaintenance = await _dbSet.FindAsync(preventiveMaintenanceId);
        if (preventiveMaintenance != null)
        {
            preventiveMaintenance.IsActive = isActive;
            preventiveMaintenance.SetUpdated(updatedBy);
            _dbSet.Update(preventiveMaintenance);
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}