using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;
using EquipTrack.Application.Users.Commands;
using EquipTrack.Application.Users.Queries;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Enums;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// API controller for technician/user management operations using CQRS pattern.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class TechniciansController : ControllerBase
{
    private readonly IMediator _mediator;

    public TechniciansController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all technicians/users with filtering and pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter users by name or email.</param>
    /// <param name="role">Filter by user role.</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="department">Filter by department.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    /// <returns>Paginated list of users.</returns>
    [HttpGet("GetTechnicians")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetTechnicians(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] UserRole? role = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? department = null,
        [FromQuery] string orderBy = "FirstName",
        [FromQuery] bool orderAscending = true)
    {
        var query = new GetUsersQuery(
            pageNumber,
            pageSize,
            searchTerm,
            role,
            isActive,
            department,
            orderBy,
            orderAscending);

        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific technician/user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The user with the specified ID.</returns>
    [HttpGet("GetById/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserQuery>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new technician/user.
    /// </summary>
    /// <param name="command">The command containing user creation data.</param>
    /// <returns>The created user ID.</returns>
    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Create([FromBody][Required] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing technician/user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="command">The command containing updated user data.</param>
    /// <returns>Success result.</returns>
    [HttpPut("Update/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody][Required] UpdateUserCommand command)
    {
        // Ensure the ID in the route matches the ID in the command
        if (id != command.Id)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "The user ID in the URL must match the ID in the request body."
            });
        }

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a technician/user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>Success result.</returns>
    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Activate a technician/user account.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Activate/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateUserCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deactivate a technician/user account.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>Success result.</returns>
    [HttpPost("Deactivate/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateUserCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Change user password.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">Password change request.</param>
    /// <returns>Success result.</returns>
    [HttpPost("ChangePassword/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody][Required] ChangePasswordRequest request)
    {
        var command = new ChangeUserPasswordCommand(id, request.NewPassword);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Assign user to work order.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">Work order assignment request.</param>
    /// <returns>Success result.</returns>
    [HttpPost("AssignToWorkOrder/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> AssignToWorkOrder(Guid id, [FromBody][Required] AssignToWorkOrderRequest request)
    {
        var command = new AssignUserToWorkOrderCommand(id, request.WorkOrderId);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get work orders assigned to a specific technician.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="status">Filter by work order status.</param>
    /// <returns>Paginated list of work orders assigned to the technician.</returns>
    [HttpGet("GetWorkOrders/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<WorkOrderQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetWorkOrders(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] WorkOrderStatus? status = null)
    {
        var query = new GetWorkOrdersByUserQuery(id, pageNumber, pageSize, status);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get technicians by role.
    /// </summary>
    /// <param name="role">User role to filter by.</param>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated list of users with the specified role.</returns>
    [HttpGet("GetByRole/{role}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetByRole(
        UserRole role,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetUsersByRoleQuery(role, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request model for changing user password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// New password for the user.
    /// </summary>
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request model for assigning user to work order.
/// </summary>
public class AssignToWorkOrderRequest
{
    /// <summary>
    /// Work order ID to assign the user to.
    /// </summary>
    [Required]
    public Guid WorkOrderId { get; set; }
}