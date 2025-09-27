using FluentValidation;
using MediatR;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Common;

namespace EquipTrack.Application.Assets.Handlers;

/// <summary>
/// Handler for processing DeleteAssetCommand requests.
/// </summary>
internal class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Result<bool>>
{
    private readonly IAssetWriteOnlyRepository _writeRepository;
    private readonly IWorkOrderReadOnlyRepository _workOrderReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteAssetCommand> _validator;

    public DeleteAssetCommandHandler(
        IAssetWriteOnlyRepository writeRepository,
        IWorkOrderReadOnlyRepository workOrderReadRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteAssetCommand> validator)
    {
        _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        _workOrderReadRepository = workOrderReadRepository ?? throw new ArgumentNullException(nameof(workOrderReadRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<bool>> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<bool>.Invalid(validationResult.AsErrors());
        }

        try
        {
            // Get the existing asset
            var asset = await _writeRepository.GetByIdAsync(request.Id);
            if (asset == null)
            {
                return Result<bool>.Error($"Asset with ID '{request.Id}' not found.");
            }

            // Check if there are any active work orders for this asset
            var hasActiveWorkOrders = await _workOrderReadRepository.HasActiveWorkOrdersForAssetAsync(request.Id);
            if (hasActiveWorkOrders)
            {
                return Result<bool>.Error("Cannot delete asset with active work orders. Please complete or cancel all work orders first.");
            }

            // Delete the asset
            _writeRepository.Delete(asset);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Error($"Failed to delete asset: {ex.Message}");
        }
    }
}