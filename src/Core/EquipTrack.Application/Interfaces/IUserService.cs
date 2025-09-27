using EquipTrack.Application.DTOs;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Interfaces;

public interface IUserService
{
    Task<Result<IEnumerable<UserQuery>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<Result<UserQuery>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<UserQuery>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<UserQuery>> CreateUserAsync(CreateUserCommand createUserDto, CancellationToken cancellationToken = default);
    Task<Result<UserQuery>> UpdateUserAsync(Guid id, UpdateUserCommand updateUserDto, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> ActivateUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}