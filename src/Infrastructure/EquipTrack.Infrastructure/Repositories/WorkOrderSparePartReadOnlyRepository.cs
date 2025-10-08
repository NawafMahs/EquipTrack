using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using System.Linq.Expressions;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for WorkOrderSparePart entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class WorkOrderSparePartReadOnlyRepository : IWorkOrderSparePartReadOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<WorkOrderSparePart> _dbSet;

    public WorkOrderSparePartReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkOrderSparePart>();
    }

    #region Base Repository Methods

    public virtual async Task<int> CountAsync(Expression<Func<WorkOrderSparePart, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<WorkOrderSparePart>> GetAllAsync()
    {
        return await _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .ToListAsync();
    }

    public virtual IQueryable<WorkOrderSparePart> GetAllQueryable()
    {
        return _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .AsQueryable();
    }

    public virtual IQueryable<TProjection> GetAll<TProjection>(Expression<Func<WorkOrderSparePart, TProjection>> selector)
    {
        return _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .Select(selector);
    }

    public virtual async Task<TProjection?> GetAsync<TProjection>(
        Expression<Func<WorkOrderSparePart, bool>> predicate, 
        Expression<Func<WorkOrderSparePart, TProjection>> selector)
    {
        return await _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .Where(predicate)
            .Select(selector)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<WorkOrderSparePart?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .FirstOrDefaultAsync(wosp => wosp.Id == id);
    }

    public virtual async Task<WorkOrderSparePart?> GetAsync(Expression<Func<WorkOrderSparePart, bool>> predicate)
    {
        return await _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<WorkOrderSparePart, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id));
    }

    #endregion

    #region WorkOrderSparePart-Specific Methods

    public async Task<List<WorkOrderSparePart>> GetByWorkOrderAsync(Guid workOrderId)
    {
        return await _dbSet
            .Include(wosp => wosp.SparePart)
            .Where(wosp => wosp.WorkOrderRef == workOrderId)
            .OrderBy(wosp => wosp.SparePart.Name)
            .ToListAsync();
    }

    public async Task<List<WorkOrderSparePart>> GetBySparePartAsync(Guid sparePartId)
    {
        return await _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Where(wosp => wosp.SparePartRef == sparePartId)
            .OrderByDescending(wosp => wosp.WorkOrder.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalQuantityUsedAsync(Guid sparePartId)
    {
        return await _dbSet
            .Where(wosp => wosp.SparePartRef == sparePartId)
            .SumAsync(wosp => wosp.QuantityUsed);
    }

    public async Task<List<WorkOrderSparePart>> GetUsageStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(wosp => wosp.WorkOrder)
            .Include(wosp => wosp.SparePart)
            .Where(wosp => wosp.WorkOrder.CreatedAt >= startDate && 
                          wosp.WorkOrder.CreatedAt <= endDate)
            .OrderByDescending(wosp => wosp.WorkOrder.CreatedAt)
            .ToListAsync();
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}