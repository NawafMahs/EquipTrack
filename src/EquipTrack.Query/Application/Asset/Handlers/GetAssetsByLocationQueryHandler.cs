using FluentValidation;
using MediatR;
using AutoMapper;
using EquipTrack.Application.Assets.Queries;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;

using EquipTrack.Application.DTOs;
namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing GetAssetsByLocationQuery requests.
/// </summary>
internal class GetAssetsByLocationQueryHandler : IRequestHandler<GetAssetsByLocationQuery, Result<PaginatedList<AssetQuery>>>
{
    private readonly IAssetReadOnlyRepository _readRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetAssetsByLocationQuery> _validator;

    public GetAssetsByLocationQueryHandler(
        IAssetReadOnlyRepository readRepository,
        IMapper mapper,
        IValidator<GetAssetsByLocationQuery> validator)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<PaginatedList<AssetQuery>>> Handle(GetAssetsByLocationQuery request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PaginatedList<AssetQuery>>.Invalid(validationResult.AsErrors());
        }

        try
        {
            // Get paginated assets by location
            var assets = await _readRepository.GetByLocationPagedAsync(
                request.Location,
                request.PageNumber,
                request.PageSize,
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
            return Result<PaginatedList<AssetQuery>>.Error($"Failed to retrieve assets by location: {ex.Message}");
        }
    }
}