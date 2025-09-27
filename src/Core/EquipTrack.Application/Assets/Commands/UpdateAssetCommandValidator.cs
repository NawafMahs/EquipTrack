using FluentValidation;

namespace EquipTrack.Application.Assets.Commands;

/// <summary>
/// Validator for UpdateAssetCommand.
/// </summary>
public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Asset ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Asset name is required.")
            .MaximumLength(200)
            .WithMessage("Asset name cannot exceed 200 characters.");

        RuleFor(x => x.AssetTag)
            .NotEmpty()
            .WithMessage("Asset tag is required.")
            .MaximumLength(50)
            .WithMessage("Asset tag cannot exceed 50 characters.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty()
            .WithMessage("Serial number is required.")
            .MaximumLength(100)
            .WithMessage("Serial number cannot exceed 100 characters.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty()
            .WithMessage("Manufacturer is required.")
            .MaximumLength(100)
            .WithMessage("Manufacturer cannot exceed 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required.")
            .MaximumLength(100)
            .WithMessage("Model cannot exceed 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required.")
            .MaximumLength(200)
            .WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.PurchaseCost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PurchaseCost.HasValue)
            .WithMessage("Purchase cost cannot be negative.");

        RuleFor(x => x.PurchaseDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.PurchaseDate.HasValue)
            .WithMessage("Purchase date cannot be in the future.");

        RuleFor(x => x.InstallationDate)
            .GreaterThanOrEqualTo(x => x.PurchaseDate)
            .When(x => x.InstallationDate.HasValue && x.PurchaseDate.HasValue)
            .WithMessage("Installation date cannot be before purchase date.");

        RuleFor(x => x.WarrantyExpirationDate)
            .GreaterThan(x => x.PurchaseDate ?? DateTime.MinValue)
            .When(x => x.WarrantyExpirationDate.HasValue && x.PurchaseDate.HasValue)
            .WithMessage("Warranty expiration date must be after purchase date.");

        RuleFor(x => x.Criticality)
            .IsInEnum()
            .WithMessage("Invalid criticality level.");
    }
}