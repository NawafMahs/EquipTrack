using Microsoft.Extensions.Logging;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

namespace EquipTrack.Infrastructure.Services;

/// <summary>
/// Authentication service implementation.
/// Note: This service is deprecated. Use CQRS pattern with MediatR commands and queries instead.
/// </summary>
[Obsolete("Use CQRS pattern with MediatR commands and queries instead.")]
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<Result<LoginQuery>> LoginAsync(LoginCommand loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AuthService is deprecated. Use CQRS pattern with LoginCommand instead.");
            return Result<LoginQuery>.Error("Use CQRS pattern with LoginCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", loginDto.Email);
            return Result<LoginQuery>.Error("An error occurred during login");
        }
    }

    public async Task<Result<UserQuery>> RegisterAsync(CreateUserCommand createUserDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AuthService is deprecated. Use CQRS pattern with RegisterCommand instead.");
            return Result<UserQuery>.Error("Use CQRS pattern with RegisterCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", createUserDto.Email);
            return Result<UserQuery>.Error("An error occurred during registration");
        }
    }

    public async Task<Result<bool>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AuthService is deprecated. Use CQRS pattern with ValidateTokenCommand instead.");
            return Result<bool>.Error("Use CQRS pattern with ValidateTokenCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return Result<bool>.Error("An error occurred while validating the token");
        }
    }

    public async Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AuthService is deprecated. Use CQRS pattern with LogoutCommand instead.");
            return Result.Error("Use CQRS pattern with LogoutCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Result.Error("An error occurred during logout");
        }
    }
}