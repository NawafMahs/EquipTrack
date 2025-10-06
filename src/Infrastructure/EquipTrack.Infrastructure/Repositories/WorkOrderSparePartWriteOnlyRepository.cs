using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for WorkOrderSparePart entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class WorkOrderSparePartWriteOnlyRepository : IWorkOrderSparePartWriteOnlyRepository
{
    private readonly EquipTrackDbContext _context;
    private readonly DbSet<WorkOrderSparePart> _dbSet;

    public WorkOrderSparePartWriteOnlyRepository(EquipTrackDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkOrderSparePart>();
    }

    #region Base Repository Methods

    public virtual async Task<WorkOrderSparePart?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual void Add(WorkOrderSparePart entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(WorkOrderSparePart entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(WorkOrderSparePart entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Delete(WorkOrderSparePart entity)
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

    public virtual void AddRange(IEnumerable<WorkOrderSparePart> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void RemoveRange(IEnumerable<WorkOrderSparePart> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task AddAsync(WorkOrderSparePart entity)
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

    public virtual async Task AddRangeAsync(IEnumerable<WorkOrderSparePart> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    #region WorkOrderSparePart-Specific Methods

    public async Task AddSparePartsToWorkOrderAsync(Guid workOrderId, IEnumerable<(Guid SparePartId, int QuantityUsed)> sparePartUsages)
    {
        var workOrderSpareParts = sparePartUsages.Select(usage => new WorkOrderSparePart
        {
            WorkOrderRef = workOrderId,
            SparePartRef = usage.SparePartId,
            QuantityUsed = usage.QuantityUsed
        });

        await _dbSet.AddRangeAsync(workOrderSpareParts);
    }

    public async Task RemoveAllSparePartsFromWorkOrderAsync(Guid workOrderId)
    {
        var workOrderSpareParts = await _dbSet
            .Where(wosp => wosp.WorkOrderRef == workOrderId)
            .ToListAsync();

        _dbSet.RemoveRange(workOrderSpareParts);
    }

    public async Task UpdateQuantityUsedAsync(Guid workOrderId, Guid sparePartId, int newQuantity)
    {
        var workOrderSparePart = await _dbSet
            .FirstOrDefaultAsync(wosp => wosp.WorkOrderRef == workOrderId && wosp.SparePartRef == sparePartId);

        if (workOrderSparePart != null)
        {
            workOrderSparePart.QuantityUsed = newQuantity;
            _dbSet.Update(workOrderSparePart);
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}