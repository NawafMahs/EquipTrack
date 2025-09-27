using FluentValidation.Results;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Extensions;

/// <summary>
/// Extension methods for validation results.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Converts FluentValidation errors to ValidationError array.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>Array of validation errors.</returns>
    public static ValidationError[] AsErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToArray();
    }
}