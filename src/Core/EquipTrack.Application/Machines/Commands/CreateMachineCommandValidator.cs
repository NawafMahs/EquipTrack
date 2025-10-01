using FluentValidation;

namespace EquipTrack.Application.Machines.Commands;

/// <summary>
/// Validator for CreateMachineCommand.
/// </summary>
public sealed class CreateMachineCommandValidator : AbstractValidator<CreateMachineCommand>
{
    public CreateMachineCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Machine name is required.")
            .MaximumLength(200).WithMessage("Machine name must not exceed 200 characters.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required.")
            .MaximumLength(100).WithMessage("Serial number must not exceed 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must not exceed 100 characters.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer is required.")
            .MaximumLength(100).WithMessage("Manufacturer must not exceed 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

        RuleFor(x => x.InstallationDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Installation date cannot be in the future.");

        RuleFor(x => x.MachineTypeRef)
            .NotEmpty().WithMessage("Machine type reference is required.");
    }
}
