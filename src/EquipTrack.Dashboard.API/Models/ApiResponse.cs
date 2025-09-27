using System.Text.Json.Serialization;


namespace EquipTrack.Dashboard.API.Models;
/// <summary>
/// Represents a standardized API response model that unifies both 
/// successful and error responses across the application.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse"/> class.
    /// Used primarily by JSON deserialization.
    /// </summary>
    [JsonConstructor]
    public ApiResponse(
        bool success,
        string? successMessage,
        int statusCode,
        IEnumerable<ApiErrorResponse>? errors)
    {
        Success = success;
        SuccessMessage = successMessage ?? string.Empty;
        StatusCode = statusCode;
        Errors = errors ?? Enumerable.Empty<ApiErrorResponse>();
    }

    /// <summary>
    /// Initializes a new empty <see cref="ApiResponse"/> instance.
    /// Useful for factory methods.
    /// </summary>
    public ApiResponse()
    {
    }

    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; protected set; }

    /// <summary>
    /// Optional message describing a successful response.
    /// </summary>
    public string SuccessMessage { get; protected set; } = string.Empty;

    /// <summary>
    /// HTTP status code associated with the response.
    /// </summary>
    public int StatusCode { get; protected set; }

    /// <summary>
    /// Collection of error details (if any).
    /// </summary>
    public IEnumerable<ApiErrorResponse> Errors { get; protected init; } = Enumerable.Empty<ApiErrorResponse>();

    #region Factory Methods

    /// <summary>
    /// Creates a successful <see cref="ApiResponse"/> with HTTP 200 (OK).
    /// </summary>
    public static ApiResponse Ok() =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK };

    /// <summary>
    /// Creates a successful <see cref="ApiResponse"/> with HTTP 200 (OK) and a success message.
    /// </summary>
    public static ApiResponse Ok(string successMessage) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    /// <summary>
    /// Creates a failed <see cref="ApiResponse"/> with HTTP 400 (Bad Request).
    /// </summary>
    public static ApiResponse BadRequest() =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest };

    public static ApiResponse BadRequest(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateErrors(errorMessage) };

    public static ApiResponse BadRequest(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    public static ApiResponse Unauthorized() =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized };

    public static ApiResponse Unauthorized(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Unauthorized(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    public static ApiResponse Forbidden() =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden };

    public static ApiResponse Forbidden(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Forbidden(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = errors };

    public static ApiResponse NotFound() =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound };

    public static ApiResponse NotFound(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateErrors(errorMessage) };

    public static ApiResponse NotFound(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    public static ApiResponse InternalServerError(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateErrors(errorMessage) };

    public static ApiResponse InternalServerError(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = errors };

    #endregion

    /// <summary>
    /// Helper method for creating a single error response.
    /// </summary>
    private static ApiErrorResponse[] CreateErrors(string errorMessage) =>
        [new ApiErrorResponse(errorMessage)];

    /// <inheritdoc/>
    public override string ToString() =>
        $"Success: {Success} | StatusCode: {StatusCode} | HasErrors: {Errors.Any()}";
}
