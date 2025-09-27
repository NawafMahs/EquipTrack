using AutoMapper;
using Microsoft.Extensions.Logging;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Assets.Entities;
using EquipTrack.Domain.Assets.Enums;

namespace EquipTrack.Infrastructure.Services;

/// <summary>
/// Asset service implementation.
/// Note: This service is deprecated. Use CQRS pattern with MediatR commands and queries instead.
/// </summary>
[Obsolete("Use CQRS pattern with MediatR commands and queries instead.")]
public class AssetService : IAssetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AssetService> _logger;

    public AssetService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AssetService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AssetQuery>>> GetAllAssetsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // This is a simplified implementation for backward compatibility
            // In a real implementation, you would use the read repository
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with GetAssetsQuery instead.");
            return Result<IEnumerable<AssetQuery>>.Success(new List<AssetQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all assets");
            return Result<IEnumerable<AssetQuery>>.Error("An error occurred while retrieving assets");
        }
    }

    public async Task<Result<AssetQuery>> GetAssetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with GetAssetByIdQuery instead.");
            return Result<AssetQuery>.NotFound($"Asset with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset {AssetId}", id);
            return Result<AssetQuery>.Error("An error occurred while retrieving the asset");
        }
    }

    public async Task<Result<IEnumerable<AssetQuery>>> GetAssetsByStatusAsync(AssetStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with GetAssetsQuery instead.");
            return Result<IEnumerable<AssetQuery>>.Success(new List<AssetQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets by status {Status}", status);
            return Result<IEnumerable<AssetQuery>>.Error("An error occurred while retrieving assets by status");
        }
    }

    public async Task<Result<IEnumerable<AssetQuery>>> GetAssetsByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with GetAssetsByLocationQuery instead.");
            return Result<IEnumerable<AssetQuery>>.Success(new List<AssetQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets by location {Location}", location);
            return Result<IEnumerable<AssetQuery>>.Error("An error occurred while retrieving assets by location");
        }
    }

    public async Task<Result<AssetQuery>> CreateAssetAsync(AssetQuery assetDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with CreateAssetCommand instead.");
            return Result<AssetQuery>.Error("Use CQRS pattern with CreateAssetCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return Result<AssetQuery>.Error("An error occurred while creating the asset");
        }
    }

    public async Task<Result<AssetQuery>> UpdateAssetAsync(Guid id, AssetQuery assetDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with UpdateAssetCommand instead.");
            return Result<AssetQuery>.Error("Use CQRS pattern with UpdateAssetCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset {AssetId}", id);
            return Result<AssetQuery>.Error("An error occurred while updating the asset");
        }
    }

    public async Task<Result> DeleteAssetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with DeleteAssetCommand instead.");
            return Result.Error("Use CQRS pattern with DeleteAssetCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset {AssetId}", id);
            return Result.Error("An error occurred while deleting the asset");
        }
    }

    public async Task<Result> UpdateAssetStatusAsync(Guid id, AssetStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with UpdateAssetStatusCommand instead.");
            return Result.Error("Use CQRS pattern with UpdateAssetStatusCommand instead.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset status for asset {AssetId}", id);
            return Result.Error("An error occurred while updating the asset status");
        }
    }

    public async Task<Result<IEnumerable<AssetQuery>>> SearchAssetsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("AssetService is deprecated. Use CQRS pattern with GetAssetsQuery instead.");
            return Result<IEnumerable<AssetQuery>>.Success(new List<AssetQuery>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching assets with term {SearchTerm}", searchTerm);
            return Result<IEnumerable<AssetQuery>>.Error("An error occurred while searching assets");
        }
    }
}