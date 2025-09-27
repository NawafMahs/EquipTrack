using FluentValidation;

namespace EquipTrack.Application.Assets.Queries;

/// <summary>
/// Validator for GetAssetByIdQuery.
/// </summary>
public class GetAssetByIdQueryValidator : AbstractValidator<GetAssetByIdQuery>
{
    public GetAssetByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Asset ID is required.");
    }
}