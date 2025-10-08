using Microsoft.EntityFrameworkCore;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Enums;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Core.SharedKernel;
using System.Linq.Expressions;

namespace EquipTrack.Infrastructure.Repositories;

/// <summary>
/// Read-only repository implementation for User entities.
/// Implements clean architecture principles with direct interface implementation.
/// </summary>
public class UserReadOnlyRepository : IUserReadOnlyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<User>();
    }

    #region Base Repository Methods

    public virtual async Task<int> CountAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<User>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual IQueryable<User> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<TProjection> GetAll<TProjection>(Expression<Func<User, TProjection>> selector)
    {
        return _dbSet.Select(selector);
    }

    public virtual async Task<TProjection?> GetAsync<TProjection>(
        Expression<Func<User, bool>> predicate, 
        Expression<Func<User, TProjection>> selector)
    {
        return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public virtual async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<User?> GetAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id));
    }

    #endregion

    #region User-Specific Methods

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Username == username);
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<List<User>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Where(u => u.Role == role)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<List<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<PaginatedList<User>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        bool? isActive = null,
        string orderBy = "Username",
        bool orderAscending = true)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(u => 
                u.Username.ToLower().Contains(lowerSearchTerm) ||
                u.Email.ToLower().Contains(lowerSearchTerm) ||
                u.FirstName.ToLower().Contains(lowerSearchTerm) ||
                u.LastName.ToLower().Contains(lowerSearchTerm));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        // Apply ordering
        query = ApplyOrdering(query, orderBy, orderAscending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<List<User>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(u => 
                u.Username.ToLower().Contains(lowerSearchTerm) ||
                u.Email.ToLower().Contains(lowerSearchTerm) ||
                u.FirstName.ToLower().Contains(lowerSearchTerm) ||
                u.LastName.ToLower().Contains(lowerSearchTerm))
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    private static IQueryable<User> ApplyOrdering(IQueryable<User> query, string orderBy, bool orderAscending)
    {
        Expression<Func<User, object>> orderExpression = orderBy.ToLower() switch
        {
            "username" => u => u.Username,
            "email" => u => u.Email,
            "firstname" => u => u.FirstName,
            "lastname" => u => u.LastName,
            "role" => u => u.Role,
            "isactive" => u => u.IsActive,
            "createdat" => u => u.CreatedWorkOrders,
            _ => u => u.Username
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