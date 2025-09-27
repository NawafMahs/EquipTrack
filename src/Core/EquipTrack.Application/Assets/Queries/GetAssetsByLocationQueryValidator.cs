using FluentValidation;

namespace EquipTrack.Application.Assets.Queries;

/// <summary>
/// Validator for GetAssetsByLocationQuery.
/// </summary>
public class GetAssetsByLocationQueryValidator : AbstractValidator<GetAssetsByLocationQuery>
{
    private readonly string[] _validOrderByFields = 
    {
        "Name", "AssetTag", "SerialNumber", "Manufacturer", "Model", 
        "Status", "Criticality", "CreatedAt", "UpdatedAt"
    };

    public GetAssetsByLocationQueryValidator()
    {
        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required.")
            .MaximumLength(200)
            .WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100.");

        RuleFor(x => x.OrderBy)
            .Must(x => _validOrderByFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"OrderBy must be one of: {string.Join(", ", _validOrderByFields)}");
    }
}