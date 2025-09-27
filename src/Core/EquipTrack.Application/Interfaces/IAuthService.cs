using EquipTrack.Application.DTOs;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Interfaces;

public interface IAuthService
{
    Task<Result<LoginQuery>> LoginAsync(LoginCommand loginDto, CancellationToken cancellationToken = default);
    Task<Result<UserQuery>> RegisterAsync(CreateUserCommand createUserDto, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default);
}