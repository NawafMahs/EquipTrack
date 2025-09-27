using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for managing preventive maintenance in the CMMS system
/// </summary>
[Authorize]
public class PreventiveMaintenanceController : BaseApiController
{
    private readonly IPreventiveMaintenanceService _preventiveMaintenanceService;
    private readonly ILogger<PreventiveMaintenanceController> _logger;

    public PreventiveMaintenanceController(
        IPreventiveMaintenanceService preventiveMaintenanceService, 
        ILogger<PreventiveMaintenanceController> logger)
    {
        _preventiveMaintenanceService = preventiveMaintenanceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all preventive maintenances
    /// </summary>
    /// <returns>List of preventive maintenances</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PreventiveMaintenanceQuery>>> GetPreventiveMaintenances()
    {
        var result = await _preventiveMaintenanceService.GetAllPreventiveMaintenancesAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific preventive maintenance by ID
    /// </summary>
    /// <param name="id">Preventive maintenance ID</param>
    /// <returns>Preventive maintenance details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PreventiveMaintenanceQuery>> GetPreventiveMaintenance(Guid id)
    {
        var result = await _preventiveMaintenanceService.GetPreventiveMaintenanceByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get preventive maintenances by asset
    /// </summary>
    /// <param name="assetId">Asset ID</param>
    /// <returns>List of preventive maintenances for the specified asset</returns>
    [HttpGet("asset/{assetId}")]
    public async Task<ActionResult<IEnumerable<PreventiveMaintenanceQuery>>> GetPreventiveMaintenancesByAsset(Guid assetId)
    {
        var result = await _preventiveMaintenanceService.GetPreventiveMaintenancesByAssetAsync(assetId);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get overdue preventive maintenances
    /// </summary>
    /// <returns>List of overdue preventive maintenances</returns>
    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<PreventiveMaintenanceQuery>>> GetOverduePreventiveMaintenances()
    {
        var result = await _preventiveMaintenanceService.GetOverduePreventiveMaintenancesAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get preventive maintenances due soon
    /// </summary>
    /// <returns>List of preventive maintenances due soon</returns>
    [HttpGet("due-soon")]
    public async Task<ActionResult<IEnumerable<PreventiveMaintenanceQuery>>> GetDueSoonPreventiveMaintenances()
    {
        var result = await _preventiveMaintenanceService.GetDueSoonPreventiveMaintenancesAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new preventive maintenance
    /// </summary>
    /// <param name="createDto">Preventive maintenance creation data</param>
    /// <returns>Created preventive maintenance</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PreventiveMaintenanceQuery>> CreatePreventiveMaintenance([FromBody] CreatePreventiveMaintenanceCommand createDto)
    {
        var result = await _preventiveMaintenanceService.CreatePreventiveMaintenanceAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetPreventiveMaintenance), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing preventive maintenance
    /// </summary>
    /// <param name="id">Preventive maintenance ID</param>
    /// <param name="updateDto">Preventive maintenance update data</param>
    /// <returns>Updated preventive maintenance</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PreventiveMaintenanceQuery>> UpdatePreventiveMaintenance(Guid id, [FromBody] UpdatePreventiveMaintenanceCommand updateDto)
    {
        var result = await _preventiveMaintenanceService.UpdatePreventiveMaintenanceAsync(id, updateDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a preventive maintenance
    /// </summary>
    /// <param name="id">Preventive maintenance ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePreventiveMaintenance(Guid id)
    {
        var result = await _preventiveMaintenanceService.DeletePreventiveMaintenanceAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Generate work order from preventive maintenance
    /// </summary>
    /// <param name="id">Preventive maintenance ID</param>
    /// <returns>Generated work order</returns>
    [HttpPost("{id}/generate-work-order")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult<WorkOrderQuery>> GenerateWorkOrderFromPreventiveMaintenance(Guid id)
    {
        var result = await _preventiveMaintenanceService.GenerateWorkOrderFromPreventiveMaintenanceAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Mark preventive maintenance as completed
    /// </summary>
    /// <param name="id">Preventive maintenance ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult> MarkAsCompleted(Guid id)
    {
        var result = await _preventiveMaintenanceService.MarkAsCompletedAsync(id);
        return result.ToActionResult();
    }
}