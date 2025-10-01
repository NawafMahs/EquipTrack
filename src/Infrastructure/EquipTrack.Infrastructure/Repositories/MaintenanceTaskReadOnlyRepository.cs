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
    private readonly EquipTrackDbContext _context;
    private readonly DbSet<MaintenanceTask> _dbSet;

    public MaintenanceTaskReadOnlyRepository(EquipTrackDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<MaintenanceTask>();
    }

    public IQueryable<MaintenanceTask> GetQueryable()
    {
        return _dbSet
            .Include(mt => mt.Asset)
            .Include(mt => mt.AssignedTechnician)
            .Include(mt => mt.CreatedBy)
            .Include(mt => mt.SpareParts)
                .ThenInclude(sp => sp.SparePart)
            .AsNoTracking();
    }

    public async Task<MaintenanceTask?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(mt => mt.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable().ToListAsync(cancellationToken);
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
        return await GetQueryable()
            .Where(mt => mt.AssetRef == assetRef)
            .OrderByDescending(mt => mt.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceTask>> GetByTechnicianRefAsync(
        Guid technicianRef,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
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
        
        return await GetQueryable()
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
        return await GetQueryable()
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


