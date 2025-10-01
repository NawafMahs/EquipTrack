using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Core.SharedKernel;
using System.Linq.Expressions;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for Asset entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class AssetReadOnlyRepository : IAssetReadOnlyRepository
{
    private readonly EquipTrackDbContext _context;
    private readonly DbSet<Asset> _dbSet;

    public AssetReadOnlyRepository(EquipTrackDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Asset>();
    }

    #region Base Repository Methods

    public virtual async Task<int> CountAsync(Expression<Func<Asset, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<Asset>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual IQueryable<Asset> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<TProjection> GetAll<TProjection>(Expression<Func<Asset, TProjection>> selector)
    {
        return _dbSet.Select(selector);
    }

    public virtual async Task<TProjection?> GetAsync<TProjection>(
        Expression<Func<Asset, bool>> predicate, 
        Expression<Func<Asset, TProjection>> selector)
    {
        return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public virtual async Task<Asset?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<Asset?> GetAsync(Expression<Func<Asset, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<Asset, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id));
    }

    #endregion

    #region Asset-Specific Methods

    public async Task<bool> ExistsByAssetTagAsync(string assetTag, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => a.AssetTag == assetTag);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => a.SerialNumber == serialNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<PaginatedList<Asset>> GetByLocationPagedAsync(
        string location,
        int pageNumber,
        int pageSize,
        string orderBy = "Name",
        bool orderAscending = true)
    {
        var query = _dbSet
            .Include(a => a.WorkOrders)
            .Where(a => a.Location.ToLower().Contains(location.ToLower()));

        query = ApplyOrdering(query, orderBy, orderAscending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<Asset>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<List<Asset>> GetByStatusAsync(AssetStatus status)
    {
        return await _dbSet
            .Include(a => a.WorkOrders)
            .Where(a => a.Status == status)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetByCriticalityAsync(AssetCriticality criticality)
    {
        return await _dbSet
            .Include(a => a.WorkOrders)
            .Where(a => a.Criticality == criticality)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<PaginatedList<Asset>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        AssetStatus? status = null,
        AssetCriticality? criticality = null,
        string? location = null,
        string? manufacturer = null,
        string orderBy = "Name",
        bool orderAscending = true)
    {
        var query = _dbSet.Include(a => a.WorkOrders).AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(a => 
                a.Name.ToLower().Contains(lowerSearchTerm) ||
                a.AssetTag.ToLower().Contains(lowerSearchTerm) ||
                a.SerialNumber.ToLower().Contains(lowerSearchTerm) ||
                a.Model.ToLower().Contains(lowerSearchTerm) ||
                a.Manufacturer.ToLower().Contains(lowerSearchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (criticality.HasValue)
        {
            query = query.Where(a => a.Criticality == criticality.Value);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(a => a.Location.ToLower().Contains(location.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(manufacturer))
        {
            query = query.Where(a => a.Manufacturer.ToLower().Contains(manufacturer.ToLower()));
        }

        // Apply ordering
        query = ApplyOrdering(query, orderBy, orderAscending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<Asset>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<List<Asset>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Include(a => a.WorkOrders)
            .Where(a => 
                a.Name.ToLower().Contains(lowerSearchTerm) ||
                a.AssetTag.ToLower().Contains(lowerSearchTerm) ||
                a.SerialNumber.ToLower().Contains(lowerSearchTerm) ||
                a.Model.ToLower().Contains(lowerSearchTerm) ||
                a.Manufacturer.ToLower().Contains(lowerSearchTerm))
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsUnderWarrantyAsync()
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .Include(a => a.WorkOrders)
            .Where(a => a.WarrantyExpiryDate.HasValue && a.WarrantyExpiryDate.Value > currentDate)
            .OrderBy(a => a.WarrantyExpiryDate)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsWithWarrantyExpiringAsync(int days)
    {
        var currentDate = DateTime.UtcNow;
        var expirationDate = currentDate.AddDays(days);
        
        return await _dbSet
            .Include(a => a.WorkOrders)
            .Where(a => a.WarrantyExpiryDate.HasValue && 
                       a.WarrantyExpiryDate.Value > currentDate &&
                       a.WarrantyExpiryDate.Value <= expirationDate)
            .OrderBy(a => a.WarrantyExpiryDate)
            .ToListAsync();
    }

    private static IQueryable<Asset> ApplyOrdering(IQueryable<Asset> query, string orderBy, bool orderAscending)
    {
        Expression<Func<Asset, object>> orderExpression = orderBy.ToLower() switch
        {
            "name" => a => a.Name,
            "assettag" => a => a.AssetTag,
            "serialnumber" => a => a.SerialNumber,
            "manufacturer" => a => a.Manufacturer,
            "model" => a => a.Model,
            "location" => a => a.Location,
            "status" => a => a.Status,
            "criticality" => a => a.Criticality,
            "createdat" => a => a.CreatedAt,
            "updatedat" => a => a.UpdatedAt,
            _ => a => a.Name
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