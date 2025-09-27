using FluentValidation;

namespace EquipTrack.Application.Assets.Commands;

/// <summary>
/// Validator for UpdateAssetStatusCommand.
/// </summary>
public class UpdateAssetStatusCommandValidator : AbstractValidator<UpdateAssetStatusCommand>
{
    public UpdateAssetStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Asset ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid asset status.");
    }
}