using System.Text.Json.Serialization;


namespace EquipTrack.Dashboard.API.Models;
/// <summary>
/// Represents a standardized API error response.
/// This model ensures consistent error serialization across endpoints.
/// </summary>
public sealed class ApiErrorResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorResponse"/> class
    /// using JSON deserialization.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    [JsonConstructor]
    public ApiErrorResponse(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    /// Gets the error message returned by the API.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc/>
    public override string ToString() => Message;
}