using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for MaintenanceTask entities.
/// Provides write operations following CQRS pattern.
/// </summary>
public sealed class MaintenanceTaskWriteOnlyRepository : IMaintenanceTaskWriteOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<MaintenanceTask> _dbSet;

    public MaintenanceTaskWriteOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<MaintenanceTask>();
    }

    public async Task<MaintenanceTask?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Add(MaintenanceTask entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(MaintenanceTask entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(MaintenanceTask entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(MaintenanceTask entity)
    {
        _dbSet.Remove(entity);
    }

    public void Remove(Guid id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public void AddRange(IEnumerable<MaintenanceTask> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void RemoveRange(IEnumerable<MaintenanceTask> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task AddAsync(MaintenanceTask entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddAsync(MaintenanceTask maintenanceTask, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(maintenanceTask, cancellationToken);
    }

    public async Task RemoveByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task AddRangeAsync(IEnumerable<MaintenanceTask> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public async Task AddRangeAsync(IEnumerable<MaintenanceTask> maintenanceTasks, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(maintenanceTasks, cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}


