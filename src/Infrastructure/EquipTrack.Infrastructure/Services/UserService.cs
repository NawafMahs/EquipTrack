using AutoMapper;
using Microsoft.Extensions.Logging;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Services;

/// <summary>
/// User service implementation.
/// Note: This service is deprecated. Use CQRS pattern with MediatR commands and queries instead.
/// </summary>
[Obsolete("Use CQRS pattern with MediatR commands and queries instead.")]
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordService passwordService,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<UserQuery>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with GetUsersQuery instead.");
            return Result<IEnumerable<UserQuery>>.Success(new List<UserQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return Result<IEnumerable<UserQuery>>.Error("An error occurred while retrieving users");
        }
    }

    public async Task<Result<UserQuery>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with GetUserByIdQuery instead.");
            return Result<UserQuery>.NotFound($"User with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return Result<UserQuery>.Error("An error occurred while retrieving the user");
        }
    }

    public async Task<Result<UserQuery>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with GetUserByEmailQuery instead.");
            return Result<UserQuery>.NotFound($"User with email {email} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email {Email}", email);
            return Result<UserQuery>.Error("An error occurred while retrieving the user");
        }
    }

    public async Task<Result<UserQuery>> CreateUserAsync(CreateUserCommand createUserDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with CreateUserCommand instead.");
            return Result<UserQuery>.Error("Use CQRS pattern with CreateUserCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Result<UserQuery>.Error("An error occurred while creating the user");
        }
    }

    public async Task<Result<UserQuery>> UpdateUserAsync(Guid id, UpdateUserCommand updateUserDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with UpdateUserCommand instead.");
            return Result<UserQuery>.Error("Use CQRS pattern with UpdateUserCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return Result<UserQuery>.Error("An error occurred while updating the user");
        }
    }

    public async Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with DeleteUserCommand instead.");
            return Result.Error("Use CQRS pattern with DeleteUserCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return Result.Error("An error occurred while deleting the user");
        }
    }

    public async Task<Result> ActivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with ActivateUserCommand instead.");
            return Result.Error("Use CQRS pattern with ActivateUserCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return Result.Error("An error occurred while activating the user");
        }
    }

    public async Task<Result> DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with DeactivateUserCommand instead.");
            return Result.Error("Use CQRS pattern with DeactivateUserCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return Result.Error("An error occurred while deactivating the user");
        }
    }

    public async Task<Result> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("UserService is deprecated. Use CQRS pattern with ChangePasswordCommand instead.");
            return Result.Error("Use CQRS pattern with ChangePasswordCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", id);
            return Result.Error("An error occurred while changing the password");
        }
    }
}