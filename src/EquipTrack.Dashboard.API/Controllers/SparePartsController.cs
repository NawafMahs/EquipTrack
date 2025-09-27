using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Dashboard.API.Extensions;
using EquipTrack.Dashboard.API.Models;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// API controller for spare parts inventory management operations using CQRS pattern.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class SparePartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SparePartsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all spare parts with filtering and pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter spare parts by name or part number.</param>
    /// <param name="category">Filter by spare part category.</param>
    /// <param name="supplier">Filter by supplier.</param>
    /// <param name="location">Filter by storage location.</param>
    /// <param name="lowStock">Filter by low stock items only.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    /// <returns>Paginated list of spare parts.</returns>
    [HttpGet("GetSpareParts")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<SparePartQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetSpareParts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null,
        [FromQuery] string? supplier = null,
        [FromQuery] string? location = null,
        [FromQuery] bool? lowStock = null,
        [FromQuery] string orderBy = "Name",
        [FromQuery] bool orderAscending = true)
    {
        var query = new GetSparePartsQuery(
            pageNumber,
            pageSize,
            searchTerm,
            category,
            supplier,
            location,
            lowStock,
            orderBy,
            orderAscending);

        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific spare part by ID.
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <returns>The spare part with the specified ID.</returns>
    [HttpGet("GetById/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SparePartQuery>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetSparePartByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new spare part.
    /// </summary>
    /// <param name="command">The command containing spare part creation data.</param>
    /// <returns>The created spare part ID.</returns>
    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Create([FromBody][Required] CreateSparePartCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing spare part.
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <param name="command">The command containing updated spare part data.</param>
    /// <returns>Success result.</returns>
    [HttpPut("Update/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody][Required] UpdateSparePartCommand command)
    {
        // Ensure the ID in the route matches the ID in the command
        if (id != command.Id)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "The spare part ID in the URL must match the ID in the request body."
            });
        }

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a spare part.
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <returns>Success result.</returns>
    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteSparePartCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Adjust spare part stock quantity.
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <param name="request">Stock adjustment request.</param>
    /// <returns>Success result.</returns>
    [HttpPost("AdjustStock/{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody][Required] AdjustStockRequest request)
    {
        var command = new AdjustSparePartStockCommand(id, request.Quantity, request.Reason);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Receive spare part stock (increase inventory).
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <param name="request">Stock receipt request.</param>
    /// <returns>Success result.</returns>
    [HttpPost("ReceiveStock/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> ReceiveStock(Guid id, [FromBody][Required] ReceiveStockRequest request)
    {
        var command = new ReceiveSparePartStockCommand(
            id, 
            request.Quantity, 
            request.UnitCost, 
            request.Supplier, 
            request.PurchaseOrderNumber,
            request.Notes);
        
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Issue spare part stock (decrease inventory for work order).
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <param name="request">Stock issue request.</param>
    /// <returns>Success result.</returns>
    [HttpPost("IssueStock/{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> IssueStock(Guid id, [FromBody][Required] IssueStockRequest request)
    {
        var command = new IssueSparePartStockCommand(
            id, 
            request.Quantity, 
            request.WorkOrderId, 
            request.Notes);
        
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get spare parts with low stock levels.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated list of spare parts with low stock.</returns>
    [HttpGet("GetLowStock")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<SparePartQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetLowStock(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetLowStockSparePartsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request model for adjusting spare part stock.
/// </summary>
public class AdjustStockRequest
{
    /// <summary>
    /// Quantity to adjust (positive for increase, negative for decrease).
    /// </summary>
    [Required]
    public int Quantity { get; set; }

    /// <summary>
    /// Reason for the stock adjustment.
    /// </summary>
    [Required]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Request model for receiving spare part stock.
/// </summary>
public class ReceiveStockRequest
{
    /// <summary>
    /// Quantity received.
    /// </summary>
    [Required]
    public int Quantity { get; set; }

    /// <summary>
    /// Unit cost of the received items.
    /// </summary>
    [Required]
    public decimal UnitCost { get; set; }

    /// <summary>
    /// Supplier name.
    /// </summary>
    public string? Supplier { get; set; }

    /// <summary>
    /// Purchase order number.
    /// </summary>
    public string? PurchaseOrderNumber { get; set; }

    /// <summary>
    /// Additional notes.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for issuing spare part stock.
/// </summary>
public class IssueStockRequest
{
    /// <summary>
    /// Quantity to issue.
    /// </summary>
    [Required]
    public int Quantity { get; set; }

    /// <summary>
    /// Work order ID for which the stock is being issued.
    /// </summary>
    [Required]
    public Guid WorkOrderId { get; set; }

    /// <summary>
    /// Additional notes.
    /// </summary>
    public string? Notes { get; set; }
}