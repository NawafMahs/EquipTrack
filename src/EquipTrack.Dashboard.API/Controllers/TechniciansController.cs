using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for technician/user management operations
/// </summary>
[Authorize]
public class TechniciansController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly ILogger<TechniciansController> _logger;

    public TechniciansController(IUserService userService, ILogger<TechniciansController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all technicians/users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    public async Task<IActionResult> GetTechnicians()
    {
        var result = await _userService.GetAllUsersAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific technician/user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTechnician(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new technician/user
    /// </summary>
    /// <param name="createDto">User creation data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateTechnician([FromBody] CreateUserCommand createDto)
    {
        var result = await _userService.CreateUserAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetTechnician), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing technician/user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateDto">User update data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateTechnician(Guid id, [FromBody] UpdateUserCommand updateDto)
    {
        var result = await _userService.UpdateUserAsync(id, updateDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a technician/user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTechnician(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result.ToActionResult();
    }
}
