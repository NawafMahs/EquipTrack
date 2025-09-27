using FluentValidation;
using MediatR;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Common;

namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing UpdateAssetStatusCommand requests.
/// </summary>
internal class UpdateAssetStatusCommandHandler : IRequestHandler<UpdateAssetStatusCommand, Result<Guid>>
{
    private readonly IAssetWriteOnlyRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateAssetStatusCommand> _validator;

    public UpdateAssetStatusCommandHandler(
        IAssetWriteOnlyRepository writeRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateAssetStatusCommand> validator)
    {
        _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<Guid>> Handle(UpdateAssetStatusCommand request, CancellationToken cancellationToken)
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

            // Update asset status
            asset.UpdateStatus(request.Status);

            // Update in repository
            _writeRepository.Update(asset);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Return the asset's ID
            return Result<Guid>.Success(asset.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Error($"Failed to update asset status: {ex.Message}");
        }
    }
}