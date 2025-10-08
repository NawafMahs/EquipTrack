using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Core.SharedKernel;
using System.Linq.Expressions;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for PreventiveMaintenance entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class PreventiveMaintenanceReadOnlyRepository : IPreventiveMaintenanceReadOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<PreventiveMaintenance> _dbSet;

    public PreventiveMaintenanceReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<PreventiveMaintenance>();
    }

    #region Base Repository Methods

    public virtual async Task<int> CountAsync(Expression<Func<PreventiveMaintenance, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<PreventiveMaintenance>> GetAllAsync()
    {
        return await _dbSet.Include(pm => pm.Asset).ToListAsync();
    }

    public virtual IQueryable<PreventiveMaintenance> GetAllQueryable()
    {
        return _dbSet.Include(pm => pm.Asset).AsQueryable();
    }

    public virtual IQueryable<TProjection> GetAll<TProjection>(Expression<Func<PreventiveMaintenance, TProjection>> selector)
    {
        return _dbSet.Include(pm => pm.Asset).Select(selector);
    }

    public virtual async Task<TProjection?> GetAsync<TProjection>(
        Expression<Func<PreventiveMaintenance, bool>> predicate, 
        Expression<Func<PreventiveMaintenance, TProjection>> selector)
    {
        return await _dbSet.Include(pm => pm.Asset).Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public virtual async Task<PreventiveMaintenance?> GetByIdAsync(Guid id)
    {
        return await _dbSet.Include(pm => pm.Asset).FirstOrDefaultAsync(pm => pm.Id == id);
    }

    public virtual async Task<PreventiveMaintenance?> GetAsync(Expression<Func<PreventiveMaintenance, bool>> predicate)
    {
        return await _dbSet.Include(pm => pm.Asset).FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<PreventiveMaintenance, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id));
    }

    #endregion

    #region PreventiveMaintenance-Specific Methods

    public async Task<List<PreventiveMaintenance>> GetByAssetAsync(Guid assetId)
    {
        return await _dbSet
            .Include(pm => pm.Asset)
            .Where(pm => pm.AssetRef == assetId)
            .OrderBy(pm => pm.NextDueDate)
            .ToListAsync();
    }

    public async Task<List<PreventiveMaintenance>> GetByFrequencyAsync(MaintenanceFrequency frequency)
    {
        return await _dbSet
            .Include(pm => pm.Asset)
            .Where(pm => pm.Frequency == frequency)
            .OrderBy(pm => pm.NextDueDate)
            .ToListAsync();
    }

    public async Task<List<PreventiveMaintenance>> GetOverdueAsync()
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .Include(pm => pm.Asset)
            .Where(pm => pm.NextDueDate < currentDate && pm.IsActive)
            .OrderBy(pm => pm.NextDueDate)
            .ToListAsync();
    }

    public async Task<List<PreventiveMaintenance>> GetDueSoonAsync(int days)
    {
        var currentDate = DateTime.UtcNow;
        var dueDate = currentDate.AddDays(days);
        
        return await _dbSet
            .Include(pm => pm.Asset)
            .Where(pm => pm.NextDueDate >= currentDate && 
                        pm.NextDueDate <= dueDate && 
                        pm.IsActive)
            .OrderBy(pm => pm.NextDueDate)
            .ToListAsync();
    }

    public async Task<PaginatedList<PreventiveMaintenance>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? assetId = null,
        MaintenanceFrequency? frequency = null,
        bool? isActive = null,
        string orderBy = "Title",
        bool orderAscending = true)
    {
        var query = _dbSet.Include(pm => pm.Asset).AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(pm => 
                pm.Name.ToLower().Contains(lowerSearchTerm) ||
                pm.Description.ToLower().Contains(lowerSearchTerm) ||
                pm.Asset.Name.ToLower().Contains(lowerSearchTerm));
        }

        if (assetId.HasValue)
        {
            query = query.Where(pm => pm.AssetRef == assetId.Value);
        }

        if (frequency.HasValue)
        {
            query = query.Where(pm => pm.Frequency == frequency.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(pm => pm.IsActive == isActive.Value);
        }

        // Apply ordering
        query = ApplyOrdering(query, orderBy, orderAscending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<PreventiveMaintenance>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<List<PreventiveMaintenance>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Include(pm => pm.Asset)
            .Where(pm => 
                pm.Name.ToLower().Contains(lowerSearchTerm) ||
                pm.Description.ToLower().Contains(lowerSearchTerm) ||
                pm.Asset.Name.ToLower().Contains(lowerSearchTerm))
            .OrderBy(pm => pm.Name)
            .ToListAsync();
    }

    private static IQueryable<PreventiveMaintenance> ApplyOrdering(IQueryable<PreventiveMaintenance> query, string orderBy, bool orderAscending)
    {
        Expression<Func<PreventiveMaintenance, object>> orderExpression = orderBy.ToLower() switch
        {
            "title" => pm => pm.Name,
            "name" => pm => pm.Name,
            "frequency" => pm => pm.Frequency,
            "nextduedate" => pm => pm.NextDueDate,
            "lastcompleteddate" => pm => pm.LastCompletedDate ?? DateTime.MinValue,
            "isactive" => pm => pm.IsActive,
            "asset" => pm => pm.Asset.Name,
            "createdat" => pm => pm.CreatedAt,
            "updatedat" => pm => pm.UpdatedAt,
            _ => pm => pm.Name
        };

        return orderAscending 
            ? query.OrderBy(orderExpression) 
            : query.OrderByDescending(orderExpression);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}