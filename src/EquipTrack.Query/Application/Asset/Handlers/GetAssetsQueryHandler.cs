using FluentValidation;
using MediatR;
using AutoMapper;
using EquipTrack.Application.Assets.Queries;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using EquipTrack.Query.QueryModels;

namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing GetAssetsQuery requests.
/// </summary>
internal class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, Result<PaginatedList<AssetQuery>>>
{
    private readonly IAssetReadOnlyRepository _readRepository;
    private readonly IValidator<GetAssetsQuery> _validator;
    private readonly IMapper _mapper;

    public GetAssetsQueryHandler(
        IAssetReadOnlyRepository readRepository,
        IValidator<GetAssetsQuery> validator,
        IMapper mapper)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PaginatedList<AssetQuery>>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PaginatedList<AssetQuery>>.Invalid(validationResult.AsErrors());
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
            var paginatedResult = new PaginatedList<AssetQuery>(
                assetDtos,
                assets.TotalCount,
                request.PageNumber,
                request.PageSize);
            
            return Result<PaginatedList<AssetQuery>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<AssetQuery>>.Error($"Failed to retrieve assets: {ex.Message}");
        }
    }
}