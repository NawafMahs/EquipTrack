using FluentValidation;

namespace EquipTrack.Application.Machines.Commands;

/// <summary>
/// Validator for UpdateMachineStatusCommand.
/// </summary>
public sealed class UpdateMachineStatusCommandValidator : AbstractValidator<UpdateMachineStatusCommand>
{
    public UpdateMachineStatusCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty().WithMessage("Machine ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid machine status.");
    }
}
