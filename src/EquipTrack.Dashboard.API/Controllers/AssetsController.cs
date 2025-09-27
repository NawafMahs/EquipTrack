using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.Assets.Queries;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Assets.Enums;
using EquipTrack.Dashboard.API.Extensions;
using System.Net;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// API controller for asset operations using CQRS pattern.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IMediator mediator, ILogger<AssetsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all assets with filtering and pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="searchTerm">Search term to filter assets by name, asset tag, or serial number.</param>
    /// <param name="status">Filter by asset status.</param>
    /// <param name="criticality">Filter by asset criticality.</param>
    /// <param name="location">Filter by asset location.</param>
    /// <param name="manufacturer">Filter by manufacturer.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    /// <returns>Paginated list of assets.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<AssetQuery>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAssets(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] AssetStatus? status = null,
        [FromQuery] AssetCriticality? criticality = null,
        [FromQuery] string? location = null,
        [FromQuery] string? manufacturer = null,
        [FromQuery] string orderBy = "Name",
        [FromQuery] bool orderAscending = true)
    {
        _logger.LogInformation("Getting assets. Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}, Status: {Status}, Criticality: {Criticality}, Location: {Location}, Manufacturer: {Manufacturer}", 
            pageNumber, pageSize, searchTerm, status, criticality, location, manufacturer);
        
        var query = new GetAssetsQuery(
            pageNumber, 
            pageSize, 
            searchTerm, 
            status, 
            criticality, 
            location, 
            manufacturer, 
            orderBy, 
            orderAscending);
        
        var result = await _mediator.Send<Result<PaginatedList<AssetQuery>>>(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific asset by ID.
    /// </summary>
    /// <param name="id">The asset ID.</param>
    /// <returns>The asset with the specified ID.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AssetQuery), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsset(Guid id)
    {
        _logger.LogInformation("Getting asset with ID {AssetId}", id);
        
        var query = new GetAssetByIdQuery(id);
        var result = await _mediator.Send<Result<AssetQuery>>(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get assets by location with pagination.
    /// </summary>
    /// <param name="location">The location to filter by.</param>
    /// <param name="pageNumber">Page number (starts from 1).</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderAscending">Order direction (true for ascending, false for descending).</param>
    /// <returns>Paginated list of assets in the specified location.</returns>
    [HttpGet("location/{location}")]
    [ProducesResponseType(typeof(PaginatedList<AssetQuery>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAssetsByLocation(
        string location,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "Name",
        [FromQuery] bool orderAscending = true)
    {
        _logger.LogInformation("Getting assets by location: {Location}", location);
        
        var query = new GetAssetsByLocationQuery(location, pageNumber, pageSize, orderBy, orderAscending);
        var result = await _mediator.Send<Result<PaginatedList<AssetQuery>>>(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new asset.
    /// </summary>
    /// <param name="command">The command containing asset creation data.</param>
    /// <returns>The created asset ID.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsset([FromBody] CreateAssetCommand command)
    {
        _logger.LogInformation("Creating new asset with name {AssetName}", command.Name);
        
        var result = await _mediator.Send<Result<Guid>>(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        // Return the ID of the newly created asset
        return CreatedAtAction(nameof(GetAsset), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update an existing asset.
    /// </summary>
    /// <param name="id">The asset ID.</param>
    /// <param name="command">The command containing updated asset data.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsset(Guid id, [FromBody] UpdateAssetCommand command)
    {
        // Ensure the ID in the route matches the ID in the command
        if (id != command.Id)
        {
            return BadRequest("The asset ID in the URL must match the ID in the request body.");
        }

        _logger.LogInformation("Updating asset with ID {AssetId}", id);
        
        var result = await _mediator.Send<Result<Guid>>(command);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(result.Errors);
            }
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Update an asset's status.
    /// </summary>
    /// <param name="id">The asset ID.</param>
    /// <param name="request">The request containing the new status.</param>
    /// <returns>No content on success.</returns>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAssetStatus(Guid id, [FromBody] UpdateAssetStatusRequest request)
    {
        _logger.LogInformation("Updating asset status for ID {AssetId} to {Status}", id, request.Status);
        
        var command = new UpdateAssetStatusCommand(id, request.Status);
        var result = await _mediator.Send<Result<Guid>>(command);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(result.Errors);
            }
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Delete an asset.
    /// </summary>
    /// <param name="id">The asset ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsset(Guid id)
    {
        _logger.LogInformation("Deleting asset with ID {AssetId}", id);
        
        var command = new DeleteAssetCommand(id);
        var result = await _mediator.Send<Result<bool>>(command);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(result.Errors);
            }
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}

/// <summary>
/// Request model for updating asset status.
/// </summary>
public class UpdateAssetStatusRequest
{
    /// <summary>
    /// New asset status.
    /// </summary>
    public AssetStatus Status { get; set; }
}