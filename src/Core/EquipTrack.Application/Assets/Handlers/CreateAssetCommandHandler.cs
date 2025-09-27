using FluentValidation;
using MediatR;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Assets.Entities;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Common;

namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing CreateAssetCommand requests.
/// </summary>
internal class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<Guid>>
{
    private readonly IAssetWriteOnlyRepository _writeRepository;
    private readonly IAssetReadOnlyRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAssetCommand> _validator;

    public CreateAssetCommandHandler(
        IAssetWriteOnlyRepository writeRepository,
        IAssetReadOnlyRepository readRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateAssetCommand> validator)
    {
        _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<Guid>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<Guid>.Invalid(validationResult.AsErrors());
        }

        try
        {
            // Check if an asset with the same asset tag already exists
            if (await _readRepository.ExistsByAssetTagAsync(request.AssetTag))
            {
                return Result<Guid>.Error($"An asset with asset tag '{request.AssetTag}' already exists.");
            }

            // Check if an asset with the same serial number already exists
            if (await _readRepository.ExistsBySerialNumberAsync(request.SerialNumber))
            {
                return Result<Guid>.Error($"An asset with serial number '{request.SerialNumber}' already exists.");
            }

            // Create new asset using the factory method
            var asset = Asset.Create(
                request.Name,
                request.Description,
                request.AssetTag,
                request.SerialNumber,
                request.Manufacturer,
                request.Model,
                request.Location,
                request.Criticality);

            // Set optional purchase information if provided
            if (request.PurchaseDate.HasValue || request.PurchaseCost.HasValue)
            {
                asset.SetPurchaseInfo(request.PurchaseDate, request.PurchaseCost);
            }

            // Set installation date if provided
            if (request.InstallationDate.HasValue)
            {
                asset.SetInstallationDate(request.InstallationDate);
            }

            // Set warranty expiration date if provided
            if (request.WarrantyExpirationDate.HasValue)
            {
                asset.SetWarrantyExpirationDate(request.WarrantyExpirationDate);
            }

            // Add to repository
            _writeRepository.Add(asset);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Return the new asset's ID
            return Result<Guid>.Success(asset.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Error($"Failed to create asset: {ex.Message}");
        }
    }
}