using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Domain.Repositories;

/// <summary>
/// Write-only repository interface for User entities.
/// </summary>
public interface IUserWriteOnlyRepository : IWriteOnlyRepository<User, int>
{
    /// <summary>
    /// Updates the user's last login timestamp.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="lastLoginAt">The last login timestamp.</param>
    Task UpdateLastLoginAsync(int userId, DateTime lastLoginAt);

    /// <summary>
    /// Updates the user's password hash.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="passwordHash">The new password hash.</param>
    Task UpdatePasswordAsync(int userId, string passwordHash);

    /// <summary>
    /// Activates or deactivates a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="isActive">The active status.</param>
    Task UpdateActiveStatusAsync(int userId, bool isActive);
}