using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for SparePart entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class SparePartWriteOnlyRepository : ISparePartWriteOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<SparePart> _dbSet;

    public SparePartWriteOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<SparePart>();
    }

    #region Base Repository Methods

    public virtual async Task<SparePart?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual void Add(SparePart entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(SparePart entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(SparePart entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Delete(SparePart entity)
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

    public virtual void AddRange(IEnumerable<SparePart> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void RemoveRange(IEnumerable<SparePart> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task AddAsync(SparePart entity)
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

    public virtual async Task AddRangeAsync(IEnumerable<SparePart> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    #region SparePart-Specific Methods

    public async Task UpdateStockQuantityAsync(Guid sparePartId, int newQuantity, string updatedBy)
    {
        var sparePart = await _dbSet.FindAsync(sparePartId);
        if (sparePart != null)
        {
            sparePart.QuantityInStock = newQuantity;
            _dbSet.Update(sparePart);
        }
    }

    public async Task DecreaseStockAsync(Guid sparePartId, int quantityUsed, string updatedBy)
    {
        var sparePart = await _dbSet.FindAsync(sparePartId);
        if (sparePart != null)
        {
            sparePart.QuantityInStock = Math.Max(0, sparePart.QuantityInStock - quantityUsed);
            _dbSet.Update(sparePart);
        }
    }

    public async Task IncreaseStockAsync(Guid sparePartId, int quantityAdded, string updatedBy)
    {
        var sparePart = await _dbSet.FindAsync(sparePartId);
        if (sparePart != null)
        {
            sparePart.QuantityInStock += quantityAdded;
            _dbSet.Update(sparePart);
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}