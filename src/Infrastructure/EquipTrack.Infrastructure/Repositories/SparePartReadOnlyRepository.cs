using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Core.SharedKernel;
using System.Linq.Expressions;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for SparePart entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class SparePartReadOnlyRepository : ISparePartReadOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<SparePart> _dbSet;

    public SparePartReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<SparePart>();
    }

    #region Base Repository Methods

    public virtual async Task<int> CountAsync(Expression<Func<SparePart, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<SparePart>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual IQueryable<SparePart> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<TProjection> GetAll<TProjection>(Expression<Func<SparePart, TProjection>> selector)
    {
        return _dbSet.Select(selector);
    }

    public virtual async Task<TProjection?> GetAsync<TProjection>(
        Expression<Func<SparePart, bool>> predicate, 
        Expression<Func<SparePart, TProjection>> selector)
    {
        return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public virtual async Task<SparePart?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<SparePart?> GetAsync(Expression<Func<SparePart, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<SparePart, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id));
    }

    #endregion

    #region SparePart-Specific Methods

    public async Task<bool> ExistsByPartNumberAsync(string partNumber, Guid? excludeId = null)
    {
        var query = _dbSet.Where(sp => sp.PartNumber == partNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(sp => sp.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<List<SparePart>> GetLowStockPartsAsync()
    {
        return await _dbSet
            .Where(sp => sp.QuantityInStock <= sp.MinimumStockLevel)
            .OrderBy(sp => sp.Name)
            .ToListAsync();
    }

    public async Task<List<SparePart>> GetBySupplierAsync(string supplier)
    {
        return await _dbSet
            .Where(sp => sp.Supplier.ToLower().Contains(supplier.ToLower()))
            .OrderBy(sp => sp.Name)
            .ToListAsync();
    }

    public async Task<PaginatedList<SparePart>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? supplier = null,
        string? category = null,
        bool? lowStock = null,
        string orderBy = "Name",
        bool orderAscending = true)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(sp => 
                sp.Name.ToLower().Contains(lowerSearchTerm) ||
                sp.PartNumber.ToLower().Contains(lowerSearchTerm) ||
                sp.Description.ToLower().Contains(lowerSearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            query = query.Where(sp => sp.Supplier.ToLower().Contains(supplier.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(sp => sp.Category.ToLower().Contains(category.ToLower()));
        }

        if (lowStock.HasValue && lowStock.Value)
        {
            query = query.Where(sp => sp.QuantityInStock <= sp.MinimumStockLevel);
        }

        // Apply ordering
        query = ApplyOrdering(query, orderBy, orderAscending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<SparePart>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<List<SparePart>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(sp => 
                sp.Name.ToLower().Contains(lowerSearchTerm) ||
                sp.PartNumber.ToLower().Contains(lowerSearchTerm) ||
                sp.Description.ToLower().Contains(lowerSearchTerm))
            .OrderBy(sp => sp.Name)
            .ToListAsync();
    }

    private static IQueryable<SparePart> ApplyOrdering(IQueryable<SparePart> query, string orderBy, bool orderAscending)
    {
        Expression<Func<SparePart, object>> orderExpression = orderBy.ToLower() switch
        {
            "name" => sp => sp.Name,
            "partnumber" => sp => sp.PartNumber,
            "supplier" => sp => sp.Supplier,
            "category" => sp => sp.Category,
            "quantityinstock" => sp => sp.QuantityInStock,
            "minimumstocklevel" => sp => sp.MinimumStockLevel,
            "unitprice" => sp => sp.UnitPrice,
            "createdat" => sp => sp.CreatedAt,
            "updatedat" => sp => sp.UpdatedAt,
            _ => sp => sp.Name
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