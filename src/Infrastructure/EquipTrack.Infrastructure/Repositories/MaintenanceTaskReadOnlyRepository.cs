using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for MaintenanceTask entities.
/// Provides optimized query operations following CQRS pattern.
/// </summary>
public sealed class MaintenanceTaskReadOnlyRepository : IMaintenanceTaskReadOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<MaintenanceTask> _dbSet;

    public MaintenanceTaskReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<MaintenanceTask>();
    }

    public IQueryable<MaintenanceTask> GetAllQueryable()
    {
        return _dbSet
            .Include(mt => mt.Asset)
            .Include(mt => mt.AssignedTechnician)
            .Include(mt => mt.CreatedBy)
            .Include(mt => mt.SpareParts)
                .ThenInclude(sp => sp.SparePart)
            .AsNoTracking();
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<MaintenanceTask, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<List<MaintenanceTask>> GetAllAsync()
    {
        return await GetAllQueryable().ToListAsync();
    }

    public IQueryable<TProjection> GetAll<TProjection>(System.Linq.Expressions.Expression<Func<MaintenanceTask, TProjection>> selector)
    {
        return _dbSet.Select(selector);
    }

    public async Task<TProjection?> GetAsync<TProjection>(
        System.Linq.Expressions.Expression<Func<MaintenanceTask, bool>> predicate,
        System.Linq.Expressions.Expression<Func<MaintenanceTask, TProjection>> selector)
    {
        return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public async Task<MaintenanceTask?> GetByIdAsync(Guid id)
    {
        return await GetAllQueryable()
            .FirstOrDefaultAsync(mt => mt.Id == id);
    }

    public async Task<MaintenanceTask?> GetAsync(System.Linq.Expressions.Expression<Func<MaintenanceTask, bool>> predicate)
    {
        return await GetAllQueryable().FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<MaintenanceTask, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _dbSet.AnyAsync(mt => mt.Id == id);
    }

    public async Task<bool> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(mt => mt.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetByAssetRefAsync(
        Guid assetRef,
        CancellationToken cancellationToken = default)
    {
        return await GetAllQueryable()
            .Where(mt => mt.AssetRef == assetRef)
            .OrderByDescending(mt => mt.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetByTechnicianRefAsync(
        int technicianRef,
        CancellationToken cancellationToken = default)
    {
        return await GetAllQueryable()
            .Where(mt => mt.AssignedTechnicianRef == technicianRef)
            .Where(mt => mt.Status != MaintenanceTaskStatus.Completed 
                      && mt.Status != MaintenanceTaskStatus.Cancelled)
            .OrderBy(mt => mt.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        return await GetAllQueryable()
            .Where(mt => mt.ScheduledDate < now)
            .Where(mt => mt.Status != MaintenanceTaskStatus.Completed 
                      && mt.Status != MaintenanceTaskStatus.Cancelled)
            .OrderBy(mt => mt.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetTasksByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await GetAllQueryable()
            .Where(mt => mt.ScheduledDate >= startDate && mt.ScheduledDate <= endDate)
            .OrderBy(mt => mt.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}


