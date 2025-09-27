using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for managing spare parts in the CMMS system
/// </summary>
[Authorize]
public class SparePartsController : BaseApiController
{
    private readonly ISparePartService _sparePartService;
    private readonly ILogger<SparePartsController> _logger;

    public SparePartsController(ISparePartService sparePartService, ILogger<SparePartsController> logger)
    {
        _sparePartService = sparePartService;
        _logger = logger;
    }

    /// <summary>
    /// Get all spare parts
    /// </summary>
    /// <returns>List of spare parts</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SparePartQuery>>> GetSpareParts()
    {
        var result = await _sparePartService.GetAllSparePartsAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific spare part by ID
    /// </summary>
    /// <param name="id">Spare part ID</param>
    /// <returns>Spare part details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SparePartQuery>> GetSparePart(Guid id)
    {
        var result = await _sparePartService.GetSparePartByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get spare parts by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>List of spare parts in the specified category</returns>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<SparePartQuery>>> GetSparePartsByCategory(string category)
    {
        var result = await _sparePartService.GetSparePartsByCategoryAsync(category);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get spare parts with low stock
    /// </summary>
    /// <returns>List of spare parts with low stock</returns>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<SparePartQuery>>> GetLowStockSpareParts()
    {
        var result = await _sparePartService.GetLowStockSparePartsAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Search spare parts
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of matching spare parts</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SparePartQuery>>> SearchSpareParts([FromQuery] string searchTerm)
    {
        var result = await _sparePartService.SearchSparePartsAsync(searchTerm);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new spare part
    /// </summary>
    /// <param name="createSparePartDto">Spare part creation data</param>
    /// <returns>Created spare part</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<SparePartQuery>> CreateSparePart([FromBody] CreateSparePartCommand createSparePartDto)
    {
        var result = await _sparePartService.CreateSparePartAsync(createSparePartDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetSparePart), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing spare part
    /// </summary>
    /// <param name="id">Spare part ID</param>
    /// <param name="updateSparePartDto">Spare part update data</param>
    /// <returns>Updated spare part</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<SparePartQuery>> UpdateSparePart(Guid id, [FromBody] UpdateSparePartCommand updateSparePartDto)
    {
        var result = await _sparePartService.UpdateSparePartAsync(id, updateSparePartDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a spare part
    /// </summary>
    /// <param name="id">Spare part ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteSparePart(Guid id)
    {
        var result = await _sparePartService.DeleteSparePartAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update spare part stock
    /// </summary>
    /// <param name="id">Spare part ID</param>
    /// <param name="updateStockDto">Stock update data</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/update-stock")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<ActionResult> UpdateStock(Guid id, [FromBody] UpdateStockCommand updateStockDto)
    {
        var result = await _sparePartService.UpdateStockAsync(id, updateStockDto);
        return result.ToActionResult();
    }
}