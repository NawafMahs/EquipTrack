namespace EquipTrack.Core.SharedKernel;

/// <summary>
/// Represents the status of a result operation.
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Ok = 200,

    /// <summary>
    /// The operation failed due to invalid input or validation errors.
    /// </summary>
    Invalid = 400,

    /// <summary>
    /// The operation failed due to unauthorized access.
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// The operation failed due to forbidden access.
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// The operation failed due to a conflict.
    /// </summary>
    Conflict = 409,

    /// <summary>
    /// The operation failed due to an error.
    /// </summary>
    Error = 500,

    /// <summary>
    /// The operation failed due to a critical error.
    /// </summary>
    CriticalError = 501,

    /// <summary>
    /// The service is temporarily unavailable.
    /// </summary>
    Unavailable = 503
}