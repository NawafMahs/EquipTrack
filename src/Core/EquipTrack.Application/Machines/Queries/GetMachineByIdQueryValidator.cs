using FluentValidation;

namespace EquipTrack.Application.Machines.Queries;

/// <summary>
/// Validator for GetMachineByIdQuery.
/// </summary>
public sealed class GetMachineByIdQueryValidator : AbstractValidator<GetMachineByIdQuery>
{
    public GetMachineByIdQueryValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty().WithMessage("Machine ID is required.");
    }
}
