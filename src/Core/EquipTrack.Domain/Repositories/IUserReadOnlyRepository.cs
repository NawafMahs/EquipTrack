using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Read-only repository interface for User entities.
/// </summary>
public interface IUserReadOnlyRepository : IReadOnlyRepository<User, Guid>
{
    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Checks if a user exists with the specified username.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <returns>True if a user exists with the specified username; otherwise, false.</returns>
    Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null);

    /// <summary>
    /// Checks if a user exists with the specified email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <returns>True if a user exists with the specified email; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    /// <param name="role">The role to filter by.</param>
    /// <returns>List of users with the specified role.</returns>
    Task<List<User>> GetByRoleAsync(UserRole role);

    /// <summary>
    /// Gets active users.
    /// </summary>
    /// <returns>List of active users.</returns>
    Task<List<User>> GetActiveUsersAsync();

    /// <summary>
    /// Gets paginated users with filtering and sorting.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter by username, email, first name, or last name.</param>
    /// <param name="role">Filter by role.</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction.</param>
    /// <returns>Paginated list of users.</returns>
    Task<PaginatedList<User>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        bool? isActive = null,
        string orderBy = "Username",
        bool orderAscending = true);

    /// <summary>
    /// Searches users by username, email, first name, or last name.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>List of matching users.</returns>
    Task<List<User>> SearchAsync(string searchTerm);
}