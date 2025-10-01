using Microsoft.AspNetCore.Mvc;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Login response with token and user details</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="createUserDto">User registration data</param>
    /// <returns>Created user details</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserCommand createUserDto)
    {
        var result = await _authService.RegisterAsync(createUserDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(Register), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="tokenRequest">Token validation request</param>
    /// <returns>Token validation result</returns>
    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] TokenValidationRequest tokenRequest)
    {
        var result = await _authService.ValidateTokenAsync(tokenRequest.Token);
        return result.ToActionResult();
    }

    /// <summary>
    /// Logout user (invalidate token)
    /// </summary>
    /// <returns>Logout confirmation</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var result = await _authService.LogoutAsync(token ?? string.Empty);
        return result.ToActionResult();
    }
}

public class TokenValidationRequest
{
    public string Token { get; set; } = string.Empty;
}