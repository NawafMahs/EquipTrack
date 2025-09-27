using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for managing work orders in the CMMS system
/// </summary>
[Authorize]
public class WorkOrdersController : BaseApiController
{
    private readonly IWorkOrderService _workOrderService;
    private readonly ILogger<WorkOrdersController> _logger;

    public WorkOrdersController(IWorkOrderService workOrderService, ILogger<WorkOrdersController> logger)
    {
        _workOrderService = workOrderService;
        _logger = logger;
    }

    /// <summary>
    /// Get all work orders
    /// </summary>
    /// <returns>List of work orders</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrderQuery>>> GetWorkOrders()
    {
        var result = await _workOrderService.GetAllWorkOrdersAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific work order by ID
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <returns>Work order details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkOrderQuery>> GetWorkOrder(Guid id)
    {
        var result = await _workOrderService.GetWorkOrderByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get work orders by asset
    /// </summary>
    /// <param name="assetId">Asset ID</param>
    /// <returns>List of work orders for the specified asset</returns>
    [HttpGet("asset/{assetId}")]
    public async Task<ActionResult<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByAsset(Guid assetId)
    {
        var result = await _workOrderService.GetWorkOrdersByAssetAsync(assetId);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get work orders by user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of work orders for the specified user</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByUser(Guid userId)
    {
        var result = await _workOrderService.GetWorkOrdersByUserAsync(userId);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get my work orders (assigned to current user)
    /// </summary>
    /// <returns>List of work orders assigned to current user</returns>
    [HttpGet("my-work-orders")]
    public async Task<ActionResult<IEnumerable<WorkOrderQuery>>> GetMyWorkOrders()
    {
        var currentUserId = GetCurrentUserId();
        var result = await _workOrderService.GetWorkOrdersByUserAsync(currentUserId);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get work orders by status
    /// </summary>
    /// <param name="status">Work order status</param>
    /// <returns>List of work orders with the specified status</returns>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByStatus(WorkOrderStatus status)
    {
        var result = await _workOrderService.GetWorkOrdersByStatusAsync(status);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get work orders by priority
    /// </summary>
    /// <param name="priority">Work order priority</param>
    /// <returns>List of work orders with the specified priority</returns>
    [HttpGet("priority/{priority}")]
    public async Task<ActionResult<IEnumerable<WorkOrderQuery>>> GetWorkOrdersByPriority(WorkOrderPriority priority)
    {
        var result = await _workOrderService.GetWorkOrdersByPriorityAsync(priority);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new work order
    /// </summary>
    /// <param name="createWorkOrderDto">Work order creation data</param>
    /// <returns>Created work order</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult<WorkOrderQuery>> CreateWorkOrder([FromBody] CreateWorkOrderCommand createWorkOrderDto)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _workOrderService.CreateWorkOrderAsync(createWorkOrderDto, currentUserId);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetWorkOrder), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing work order
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <param name="updateWorkOrderDto">Work order update data</param>
    /// <returns>Updated work order</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult<WorkOrderQuery>> UpdateWorkOrder(Guid id, [FromBody] UpdateWorkOrderCommand updateWorkOrderDto)
    {
        var result = await _workOrderService.UpdateWorkOrderAsync(id, updateWorkOrderDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a work order
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> DeleteWorkOrder(Guid id)
    {
        var result = await _workOrderService.DeleteWorkOrderAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Assign work order to a user
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <param name="userId">User ID to assign to</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> AssignWorkOrder(Guid id, [FromBody] Guid userId)
    {
        var result = await _workOrderService.AssignWorkOrderAsync(id, userId);
        return result.ToActionResult();
    }

    /// <summary>
    /// Start a work order
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/start")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult> StartWorkOrder(Guid id)
    {
        var result = await _workOrderService.StartWorkOrderAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Complete a work order
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <param name="completionRequest">Completion details</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult> CompleteWorkOrder(Guid id, [FromBody] CompleteWorkOrderRequest completionRequest)
    {
        var result = await _workOrderService.CompleteWorkOrderAsync(
            id, 
            completionRequest.CompletionNotes, 
            completionRequest.ActualHours, 
            completionRequest.ActualCost);
        return result.ToActionResult();
    }

    /// <summary>
    /// Add spare part to work order
    /// </summary>
    /// <param name="id">Work order ID</param>
    /// <param name="sparePartRequest">Spare part details</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/spare-parts")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult> AddSparePartToWorkOrder(Guid id, [FromBody] AddSparePartRequest sparePartRequest)
    {
        var result = await _workOrderService.AddSparePartToWorkOrderAsync(
            id,
            sparePartRequest.SparePartId,
            sparePartRequest.Quantity,
            sparePartRequest.UnitCost,
            sparePartRequest.Notes);
        return result.ToActionResult();
    }
}

public class CompleteWorkOrderRequest
{
    public string? CompletionNotes { get; set; }
    public decimal ActualHours { get; set; }
    public decimal ActualCost { get; set; }
}

public class AddSparePartRequest
{
    public Guid SparePartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? Notes { get; set; }
}