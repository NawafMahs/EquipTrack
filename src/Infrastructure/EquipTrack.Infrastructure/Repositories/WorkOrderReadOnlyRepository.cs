using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Enums;
using EquipTrack.Infrastructure.Data;
using System.Linq.Expressions;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for WorkOrder entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class WorkOrderReadOnlyRepository : IWorkOrderReadOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<WorkOrder> _dbSet;

    public WorkOrderReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkOrder>();
    }

    #region Base Repository Methods

    public virtual async Task<int> CountAsync(Expression<Func<WorkOrder, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<WorkOrder>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual IQueryable<WorkOrder> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<TProjection> GetAll<TProjection>(Expression<Func<WorkOrder, TProjection>> selector)
    {
        return _dbSet.Select(selector);
    }

    public virtual async Task<TProjection?> GetAsync<TProjection>(
        Expression<Func<WorkOrder, bool>> predicate, 
        Expression<Func<WorkOrder, TProjection>> selector)
    {
        return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public virtual async Task<WorkOrder?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<WorkOrder?> GetAsync(Expression<Func<WorkOrder, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<WorkOrder, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id));
    }

    #endregion

    #region WorkOrder-Specific Methods

    public async Task<List<WorkOrder>> GetByStatusAsync(WorkOrderStatus status)
    {
        return await _dbSet
            .Where(wo => wo.Status == status)
            .OrderByDescending(wo => wo.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetByPriorityAsync(WorkOrderPriority priority)
    {
        return await _dbSet
            .Where(wo => wo.Priority == priority)
            .OrderByDescending(wo => wo.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetByAssignedUserAsync(Guid userRef)
    {
        return await _dbSet
            .Where(wo => wo.AssignedToUserRef == userRef)
            .OrderByDescending(wo => wo.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetByAssetAsync(Guid assetRef)
    {
        return await _dbSet
            .Where(wo => wo.AssetRef == assetRef)
            .OrderByDescending(wo => wo.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasActiveWorkOrdersForAssetAsync(Guid assetRef)
    {
        return await _dbSet
            .AnyAsync(wo => wo.AssetRef == assetRef && 
                           wo.Status != WorkOrderStatus.Completed && 
                           wo.Status != WorkOrderStatus.Cancelled);
    }

    public async Task<List<WorkOrder>> GetOverdueAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(wo => wo.ScheduledDate.HasValue && 
                        wo.ScheduledDate.Value.Date < today && 
                        wo.Status != WorkOrderStatus.Completed && 
                        wo.Status != WorkOrderStatus.Cancelled)
            .OrderBy(wo => wo.ScheduledDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(wo => 
                wo.Title.ToLower().Contains(lowerSearchTerm) ||
                (wo.Description != null && wo.Description.ToLower().Contains(lowerSearchTerm)))
            .OrderByDescending(wo => wo.CreatedAt)
            .ToListAsync();
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}