using FluentValidation;
using MediatR;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Common;

namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing UpdateAssetCommand requests.
/// </summary>
internal class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Result<Guid>>
{
    private readonly IAssetWriteOnlyRepository _writeRepository;
    private readonly IAssetReadOnlyRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateAssetCommand> _validator;

    public UpdateAssetCommandHandler(
        IAssetWriteOnlyRepository writeRepository,
        IAssetReadOnlyRepository readRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateAssetCommand> validator)
    {
        _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<Guid>> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<Guid>.Invalid(validationResult.AsErrors());
        }

        try
        {
            // Get the existing asset
            var asset = await _writeRepository.GetByIdAsync(request.Id);
            if (asset == null)
            {
                return Result<Guid>.Error($"Asset with ID '{request.Id}' not found.");
            }

            // Check if another asset with the same asset tag already exists (excluding current asset)
            if (await _readRepository.ExistsByAssetTagAsync(request.AssetTag, request.Id))
            {
                return Result<Guid>.Error($"Another asset with asset tag '{request.AssetTag}' already exists.");
            }

            // Check if another asset with the same serial number already exists (excluding current asset)
            if (await _readRepository.ExistsBySerialNumberAsync(request.SerialNumber, request.Id))
            {
                return Result<Guid>.Error($"Another asset with serial number '{request.SerialNumber}' already exists.");
            }

            // Update asset properties
            asset.SetName(request.Name);
            asset.SetDescription(request.Description);
            asset.SetAssetTag(request.AssetTag);
            asset.SetSerialNumber(request.SerialNumber);
            asset.SetManufacturer(request.Manufacturer);
            asset.SetModel(request.Model);
            asset.UpdateLocation(request.Location);
            asset.SetCriticality(request.Criticality);

            // Update optional purchase information
            asset.SetPurchaseInfo(request.PurchaseDate, request.PurchaseCost);

            // Update installation date
            asset.SetInstallationDate(request.InstallationDate);

            // Update warranty expiration date
            asset.SetWarrantyExpirationDate(request.WarrantyExpirationDate);

            // Update in repository
            _writeRepository.Update(asset);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Return the asset's ID
            return Result<Guid>.Success(asset.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Error($"Failed to update asset: {ex.Message}");
        }
    }
}