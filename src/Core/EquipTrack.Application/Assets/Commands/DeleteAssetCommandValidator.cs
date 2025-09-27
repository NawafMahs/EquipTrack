using FluentValidation;

namespace EquipTrack.Application.Assets.Commands;

/// <summary>
/// Validator for DeleteAssetCommand.
/// </summary>
public class DeleteAssetCommandValidator : AbstractValidator<DeleteAssetCommand>
{
    public DeleteAssetCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Asset ID is required.");
    }
}