using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for maintenance schedule operations
/// </summary>
[Authorize]
public class MaintenanceSchedulesController : BaseApiController
{
    private readonly IPreventiveMaintenanceService _preventiveMaintenanceService;
    private readonly ILogger<MaintenanceSchedulesController> _logger;

    public MaintenanceSchedulesController(
        IPreventiveMaintenanceService preventiveMaintenanceService, 
        ILogger<MaintenanceSchedulesController> _logger)
    {
        _preventiveMaintenanceService = preventiveMaintenanceService;
        this._logger = _logger;
    }

    /// <summary>
    /// Get all maintenance schedules
    /// </summary>
    /// <returns>List of maintenance schedules</returns>
    [HttpGet]
    public async Task<IActionResult> GetMaintenanceSchedules()
    {
        var result = await _preventiveMaintenanceService.GetAllPreventiveMaintenancesAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific maintenance schedule by ID
    /// </summary>
    /// <param name="id">Maintenance schedule ID</param>
    /// <returns>Maintenance schedule details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMaintenanceSchedule(Guid id)
    {
        var result = await _preventiveMaintenanceService.GetPreventiveMaintenanceByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new maintenance schedule
    /// </summary>
    /// <param name="createDto">Maintenance schedule creation data</param>
    /// <returns>Created maintenance schedule</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateMaintenanceSchedule([FromBody] CreatePreventiveMaintenanceCommand createDto)
    {
        var result = await _preventiveMaintenanceService.CreatePreventiveMaintenanceAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetMaintenanceSchedule), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing maintenance schedule
    /// </summary>
    /// <param name="id">Maintenance schedule ID</param>
    /// <param name="updateDto">Maintenance schedule update data</param>
    /// <returns>Updated maintenance schedule</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateMaintenanceSchedule(Guid id, [FromBody] UpdatePreventiveMaintenanceCommand updateDto)
    {
        var result = await _preventiveMaintenanceService.UpdatePreventiveMaintenanceAsync(id, updateDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a maintenance schedule
    /// </summary>
    /// <param name="id">Maintenance schedule ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMaintenanceSchedule(Guid id)
    {
        var result = await _preventiveMaintenanceService.DeletePreventiveMaintenanceAsync(id);
        return result.ToActionResult();
    }
}
