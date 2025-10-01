using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Domain.Enums;
using EquipTrack.Dashboard.API.Extensions;

namespace EquipTrack.Dashboard.API.Controllers;

/// <summary>
/// Controller for asset management operations
/// </summary>
[Authorize]
public class AssetsController : BaseApiController
{
    private readonly IAssetService _assetService;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IAssetService assetService, ILogger<AssetsController> logger)
    {
        _assetService = assetService;
        _logger = logger;
    }

    /// <summary>
    /// Get all assets
    /// </summary>
    /// <returns>List of assets</returns>
    [HttpGet]
    public async Task<IActionResult> GetAssets()
    {
        var result = await _assetService.GetAllAssetsAsync();
        return result.ToActionResult();
    }

    /// <summary>
    /// Get a specific asset by ID
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <returns>Asset details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsset(Guid id)
    {
        var result = await _assetService.GetAssetByIdAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create a new asset
    /// </summary>
    /// <param name="createDto">Asset creation data</param>
    /// <returns>Created asset</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateAsset([FromBody] AssetQuery createDto)
    {
        var result = await _assetService.CreateAssetAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetAsset), new { id = result.Value.Id }, result.Value);
        }
        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="updateDto">Asset update data</param>
    /// <returns>Updated asset</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateAsset(Guid id, [FromBody] AssetQuery updateDto)
    {
        var result = await _assetService.UpdateAssetAsync(id, updateDto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete an asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsset(Guid id)
    {
        var result = await _assetService.DeleteAssetAsync(id);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update asset status
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="request">Status update request</param>
    /// <returns>Updated asset</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<IActionResult> UpdateAssetStatus(Guid id, [FromBody] UpdateAssetStatusRequest request)
    {
        var result = await _assetService.UpdateAssetStatusAsync(id, request.Status);
        return result.ToActionResult();
    }
}

public class UpdateAssetStatusRequest
{
    public AssetStatus Status { get; set; }
}
