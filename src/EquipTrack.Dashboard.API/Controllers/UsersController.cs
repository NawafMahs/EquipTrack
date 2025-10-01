using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for managing users in the CMMS system
/// </summary>
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User details</returns>
    [HttpGet("email/{email}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await _userService.GetUserByEmailAsync(email);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.GetUserByIdAsync(currentUserId);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="createUserDto">User creation data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand createUserDto)
    {
        var result = await _userService.CreateUserAsync(createUserDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetUser), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateUserDto">User update data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand updateUserDto)
    {
        var result = await _userService.UpdateUserAsync(id, updateUserDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Activate a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var result = await _userService.ActivateUserAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deactivate a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var result = await _userService.DeactivateUserAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="changePasswordRequest">Password change data</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.ChangePasswordAsync(
            currentUserId, 
            changePasswordRequest.CurrentPassword, 
            changePasswordRequest.NewPassword);
        return result.ToActionResult();
    }

    /// <summary>
    /// Reset user password (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="resetPasswordRequest">Password reset data</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        // For admin reset, we'll use a dummy current password since admin can reset without knowing current password
        var result = await _userService.ChangePasswordAsync(id, "dummy", resetPasswordRequest.NewPassword);
        return result.ToActionResult();
    }
}


public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}