using FluentValidation;

namespace EquipTrack.Application.Assets.Queries;

/// <summary>
/// Validator for GetAssetsQuery.
/// </summary>
public class GetAssetsQueryValidator : AbstractValidator<GetAssetsQuery>
{
    private readonly string[] _validOrderByFields = 
    {
        "Name", "AssetTag", "SerialNumber", "Manufacturer", "Model", 
        "Location", "Status", "Criticality", "CreatedAt", "UpdatedAt"
    };

    public GetAssetsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term cannot exceed 200 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Location))
            .WithMessage("Location filter cannot exceed 200 characters.");

        RuleFor(x => x.Manufacturer)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Manufacturer))
            .WithMessage("Manufacturer filter cannot exceed 100 characters.");

        RuleFor(x => x.OrderBy)
            .Must(x => _validOrderByFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"OrderBy must be one of: {string.Join(", ", _validOrderByFields)}");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid asset status.");

        RuleFor(x => x.Criticality)
            .IsInEnum()
            .When(x => x.Criticality.HasValue)
            .WithMessage("Invalid asset criticality.");
    }
}