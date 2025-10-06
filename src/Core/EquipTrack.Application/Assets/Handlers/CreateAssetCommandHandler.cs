using FluentValidation;
using MediatR;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.Extensions;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Repositories;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Entities;

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
            // Asset is an abstract class and cannot be instantiated directly.
            // Use specific asset type commands (CreateMachineCommand, CreateRobotCommand, etc.)
            return Result<Guid>.Error("Cannot create generic Asset. Use specific asset type commands (CreateMachineCommand, CreateRobotCommand, etc.).");
        }
        catch (Exception ex)
        {
            return Result<Guid>.Error($"Failed to create asset: {ex.Message}");
        }
    }
}