using AutoMapper;
using EquipTrack.Application.Assets.Queries;
using EquipTrack.Application.DTOs;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EquipTrack.Query.Application.Asset.Handlers;

/// <summary>
/// Handler for processing GetAssetByIdQuery requests.
/// </summary>
internal class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, Result<AssetQuery>>
{
    private readonly IAssetReadOnlyRepository _readRepository;
    private readonly IValidator<GetAssetByIdQuery> _validator;
    private readonly IMapper _mapper;

    public GetAssetByIdQueryHandler(
        IAssetReadOnlyRepository readRepository,
        IValidator<GetAssetByIdQuery> validator,
        IMapper mapper)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<AssetQuery>> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<AssetQuery>.Invalid(validationResult.AsErrors());
        }

        try
        {
            // Get the asset
            var asset = await _readRepository.GetByIdAsync(request.Id);
            if (asset == null)
            {
                return Result<AssetQuery>.Error($"Asset with ID '{request.Id}' not found.");
            }

            // Map to DTO
            var assetDto = _mapper.Map<AssetQuery>(asset);
            
            return Result<AssetQuery>.Success(assetDto);
        }
        catch (Exception ex)
        {
            return Result<AssetQuery>.Error($"Failed to retrieve asset: {ex.Message}");
        }
    }
}