using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Enums;
using EquipTrack.Dashboard.API.Extensions;
using EquipTrack.Dashboard.API.Models;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// API controller for maintenance schedule management operations using CQRS pattern.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class MaintenanceSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MaintenanceSchedulesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all maintenance schedules with filtering and pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter maintenance schedules.</param>
    /// <param name="frequency">Filter by maintenance frequency.</param>
    /// <param name="assetId">Filter by asset ID.</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    /// <returns>Paginated list of maintenance schedules.</returns>
    [HttpGet("GetMaintenanceSchedules")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PreventiveMaintenanceQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetMaintenanceSchedules(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] MaintenanceFrequency? frequency = null,
        [FromQuery] Guid? assetId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string orderBy = "Name",
        [FromQuery] bool orderAscending = true)
    {
        var query = new GetPreventiveMaintenancesQuery(
            pageNumber,
            pageSize,
            searchTerm,
            frequency,
            assetId,
            isActive,
            orderBy,
            orderAscending);

        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific maintenance schedule by ID.
    /// </summary>
    /// <param name="id">The maintenance schedule ID.</param>
    /// <returns>The maintenance schedule with the specified ID.</returns>
    [HttpGet("GetById/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PreventiveMaintenanceQuery>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPreventiveMaintenanceByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new maintenance schedule.
    /// </summary>
    /// <param name="command">The command containing maintenance schedule creation data.</param>
    /// <returns>The created maintenance schedule ID.</returns>
    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Create([FromBody][Required] CreatePreventiveMaintenanceCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing maintenance schedule.
    /// </summary>
    /// <param name="id">The maintenance schedule ID.</param>
    /// <param name="command">The command containing updated maintenance schedule data.</param>
    /// <returns>Success result.</returns>
    [HttpPut("Update/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody][Required] UpdatePreventiveMaintenanceCommand command)
    {
       

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a maintenance schedule.
    /// </summary>
    /// <param name="id">The maintenance schedule ID.</param>
    /// <returns>Success result.</returns>
    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeletePreventiveMaintenanceCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Activate a maintenance schedule.
    /// </summary>
    /// <param name="id">The maintenance schedule ID.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Activate/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivatePreventiveMaintenanceCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deactivate a maintenance schedule.
    /// </summary>
    /// <param name="id">The maintenance schedule ID.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Deactivate/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivatePreventiveMaintenanceCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Generate work orders from due maintenance schedules.
    /// </summary>
    /// <param name="request">Request containing optional date range.</param>
    /// <returns>Number of work orders generated.</returns>
    [HttpPost("GenerateWorkOrders")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GenerateWorkOrders([FromBody] GenerateWorkOrdersRequest? request = null)
    {
        var command = new GenerateWorkOrdersFromMaintenanceCommand(
            request?.StartDate,
            request?.EndDate);
        
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request model for generating work orders from maintenance schedules.
/// </summary>
public class GenerateWorkOrdersRequest
{
    /// <summary>
    /// Start date for generating work orders (optional).
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for generating work orders (optional).
    /// </summary>
    public DateTime? EndDate { get; set; }
}