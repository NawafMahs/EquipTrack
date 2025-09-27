namespace EquipTrack.Core.SharedKernel;

/// <summary>
/// Represents a result interface for operations.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the result status.
    /// </summary>
    ResultStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the success message.
    /// </summary>
    string SuccessMessage { get; }

    /// <summary>
    /// Gets the correlation ID for tracking.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the collection of error messages.
    /// </summary>
    IEnumerable<string> Errors { get; }

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    List<ValidationError> ValidationErrors { get; }

    /// <summary>
    /// Gets the property-specific errors.
    /// </summary>
    IDictionary<string, List<string>> PropertyErrors { get; }

    /// <summary>
    /// Gets the type of the value contained in the result.
    /// </summary>
    Type ValueType { get; }

    /// <summary>
    /// Gets the value as an object.
    /// </summary>
    /// <returns>The value contained in the result.</returns>
    object GetValue();
}