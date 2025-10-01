using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;
using EquipTrack.Dashboard.API.Models;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// API controller for spare parts inventory management operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class SparePartsController : ControllerBase
{
    private readonly ISparePartService _sparePartService;

    public SparePartsController(ISparePartService sparePartService)
    {
        _sparePartService = sparePartService ?? throw new ArgumentNullException(nameof(sparePartService));
    }

    /// <summary>
    /// Get all spare parts.
    /// </summary>
    /// <param name="searchTerm">Search term to filter spare parts by name or part number.</param>
    /// <param name="category">Filter by spare part category.</param>
    /// <returns>List of spare parts.</returns>
    [HttpGet("GetSpareParts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SparePartQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetSpareParts(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null)
    {
        Result<IEnumerable<SparePartQuery>> result;
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            result = await _sparePartService.SearchSparePartsAsync(searchTerm);
        }
        else if (!string.IsNullOrEmpty(category))
        {
            result = await _sparePartService.GetSparePartsByCategoryAsync(category);
        }
        else
        {
            result = await _sparePartService.GetAllSparePartsAsync();
        }
        
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
        var result = await _sparePartService.GetSparePartByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new spare part.
    /// </summary>
    /// <param name="command">The command containing spare part creation data.</param>
    /// <returns>The created spare part.</returns>
    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<SparePartQuery>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Create([FromBody][Required] CreateSparePartCommand command)
    {
        var result = await _sparePartService.CreateSparePartAsync(command);
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
    [ProducesResponseType(typeof(ApiResponse<SparePartQuery>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody][Required] UpdateSparePartCommand command)
    {
        var result = await _sparePartService.UpdateSparePartAsync(id, command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a spare part.
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <returns>Success result.</returns>
    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sparePartService.DeleteSparePartAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update spare part stock.
    /// </summary>
    /// <param name="id">The spare part ID.</param>
    /// <param name="command">Stock update command.</param>
    /// <returns>Success result.</returns>
    [HttpPost("UpdateStock/{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody][Required] UpdateStockCommand command)
    {
        var result = await _sparePartService.UpdateStockAsync(id, command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get spare parts with low stock levels.
    /// </summary>
    /// <returns>List of spare parts with low stock.</returns>
    [HttpGet("GetLowStock")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SparePartQuery>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetLowStock()
    {
        var result = await _sparePartService.GetLowStockSparePartsAsync();
        return result.ToActionResult();
    }
}