using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;
using EquipTrack.Application.WorkOrders.Commands;
using EquipTrack.Application.WorkOrders.Queries;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Enums;
using EquipTrack.Dashboard.API.Extensions;
using EquipTrack.Dashboard.API.Models;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// API controller for work order management operations using CQRS pattern.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkOrdersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all work orders with filtering and pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter work orders.</param>
    /// <param name="status">Filter by work order status.</param>
    /// <param name="priority">Filter by work order priority.</param>
    /// <param name="assetId">Filter by asset ID.</param>
    /// <param name="assignedToUserId">Filter by assigned user ID.</param>
    /// <returns>Paginated list of work orders.</returns>
    [HttpGet("GetWorkOrders")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<WorkOrderQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetWorkOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] WorkOrderPriority? priority = null,
        [FromQuery] Guid? assetId = null,
        [FromQuery] Guid? assignedToUserId = null)
    {
        var query = new GetWorkOrdersQuery(
            status,
            priority,
            assetId,
            assignedToUserId,
            pageNumber,
            pageSize,
            searchTerm);

        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific work order by ID.
    /// </summary>
    /// <param name="id">The work order ID.</param>
    /// <returns>The work order with the specified ID.</returns>
    [HttpGet("GetById/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WorkOrderQuery>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetWorkOrderByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new work order.
    /// </summary>
    /// <param name="command">The command containing work order creation data.</param>
    /// <returns>The created work order ID.</returns>
    [HttpPost("CreateWorkOrder")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> CreateWorkOrder([FromBody][Required] CreateWorkOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing work order.
    /// </summary>
    /// <param name="id">The work order ID.</param>
    /// <param name="command">The command containing updated work order data.</param>
    /// <returns>Success result.</returns>
    [HttpPut("Update/{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody][Required] UpdateWorkOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a work order.
    /// </summary>
    /// <param name="id">The work order ID.</param>
    /// <returns>Success result.</returns>
    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteWorkOrderCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Assign work order to a user.
    /// </summary>
    /// <param name="id">Work order ID.</param>
    /// <param name="request">Assignment request containing user ID.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Assign/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Assign(Guid id, [FromBody][Required] AssignWorkOrderRequest request)
    {
        var command = new AssignWorkOrderCommand(id, request.UserId);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Start a work order.
    /// </summary>
    /// <param name="id">Work order ID.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Start/{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Start(Guid id)
    {
        var command = new StartWorkOrderCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Complete a work order.
    /// </summary>
    /// <param name="id">Work order ID.</param>
    /// <param name="request">Completion request with details.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Complete/{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Complete(Guid id, [FromBody][Required] CompleteWorkOrderRequest request)
    {
        var command = new CompleteWorkOrderCommand(id, request.CompletionNotes, request.ActualHours, request.ActualCost);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request model for assigning work order to a user.
/// </summary>
public class AssignWorkOrderRequest
{
    /// <summary>
    /// User ID to assign the work order to.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
}

/// <summary>
/// Request model for completing a work order.
/// </summary>
public class CompleteWorkOrderRequest
{
    /// <summary>
    /// Completion notes.
    /// </summary>
    public string? CompletionNotes { get; set; }

    /// <summary>
    /// Actual hours spent on the work order.
    /// </summary>
    [Required]
    public decimal ActualHours { get; set; }

    /// <summary>
    /// Actual cost of the work order.
    /// </summary>
    [Required]
    public decimal ActualCost { get; set; }
}