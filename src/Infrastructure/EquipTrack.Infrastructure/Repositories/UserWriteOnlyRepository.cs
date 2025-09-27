using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Write-only repository implementation for User entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class UserWriteOnlyRepository : IUserWriteOnlyRepository
{
    private readonly EquipTrackDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserWriteOnlyRepository(EquipTrackDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<User>();
    }

    #region Query Methods (for update operations)

    public virtual async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    #endregion

    #region Synchronous Methods

    public virtual void Add(User entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(User entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(User entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Delete(User entity)
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

    public virtual void AddRange(IEnumerable<User> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void RemoveRange(IEnumerable<User> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    #endregion

    #region Asynchronous Methods

    public virtual async Task AddAsync(User entity)
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

    public virtual async Task AddRangeAsync(IEnumerable<User> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    #endregion

    #region User-Specific Methods

    public async Task UpdateLastLoginAsync(Guid userId, DateTime lastLoginAt)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = lastLoginAt;
            user.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(user);
        }
    }

    public async Task UpdatePasswordAsync(Guid userId, string passwordHash)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.PasswordHash = passwordHash;
            user.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(user);
        }
    }

    public async Task UpdateActiveStatusAsync(Guid userId, bool isActive)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(user);
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}