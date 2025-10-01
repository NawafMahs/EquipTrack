using FluentValidation;
using MediatR;
using EquipTrack.Application.Assets.Queries;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using EquipTrack.Query.QueryModels;

namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing GetAssetsQuery requests.
/// </summary>
internal class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, Result<PaginatedList<AssetQueryModel>>>
{
    private readonly IAssetReadOnlyRepository _readRepository;
    private readonly IValidator<GetAssetsQuery> _validator;

    public GetAssetsQueryHandler(
        IAssetReadOnlyRepository readRepository,
        IValidator<GetAssetsQuery> validator)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<PaginatedList<AssetQueryModel>>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PaginatedList<AssetQueryModel>>.Invalid(validationResult.AsErrors());
        }

        try
        {
            // Get paginated assets
            var assets = await _readRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.Status,
                request.Criticality,
                request.Location,
                request.Manufacturer,
                request.OrderBy,
                request.OrderAscending);

            // Map to DTOs
            var assetDtos = _mapper.Map<List<AssetQuery>>(assets.Items);
            
            // Create paginated result
            var paginatedResult = new PaginatedList<AssetQueryModel>(
                assetDtos,
                assets.TotalCount,
                request.PageNumber,
                request.PageSize);
            
            return Result<PaginatedList<AssetQueryModel>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<AssetQueryModel>>.Error($"Failed to retrieve assets: {ex.Message}");
        }
    }
}