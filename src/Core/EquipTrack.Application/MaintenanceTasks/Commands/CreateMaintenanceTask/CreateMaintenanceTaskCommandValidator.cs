using FluentValidation;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Application.MaintenanceTasks.Commands.CreateMaintenanceTask;

/// <summary>
/// Validator for CreateMaintenanceTaskCommand.
/// Ensures all business rules are met before command execution.
/// </summary>
public sealed class CreateMaintenanceTaskCommandValidator : AbstractValidator<CreateMaintenanceTaskCommand>
{
    public CreateMaintenanceTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid maintenance task type");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority level");

        RuleFor(x => x.AssetRef)
            .NotEmpty().WithMessage("Asset reference is required");

        RuleFor(x => x.CreatedByRef)
            .NotEmpty().WithMessage("Created by reference is required");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Scheduled date cannot be in the past")
            .LessThan(DateTime.UtcNow.AddYears(2))
                .WithMessage("Scheduled date cannot be more than 2 years in the future");

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated hours cannot be negative")
            .LessThanOrEqualTo(1000).WithMessage("Estimated hours seems too high (max 1000 hours)");

        RuleFor(x => x.EstimatedCost)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated cost cannot be negative")
            .LessThanOrEqualTo(1000000).WithMessage("Estimated cost seems too high (max 1,000,000)");

        // Business rule: Emergency tasks must be scheduled within 24 hours
        When(x => x.Priority == MaintenanceTaskPriority.Emergency, () =>
        {
            RuleFor(x => x.ScheduledDate)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("Emergency tasks must be scheduled within 24 hours");
        });

        // Business rule: Preventive maintenance should be scheduled in advance
        When(x => x.Type == MaintenanceTaskType.Preventive, () =>
        {
            RuleFor(x => x.ScheduledDate)
                .GreaterThan(DateTime.UtcNow.AddDays(1))
                .WithMessage("Preventive maintenance should be scheduled at least 1 day in advance");
        });
    }
}


