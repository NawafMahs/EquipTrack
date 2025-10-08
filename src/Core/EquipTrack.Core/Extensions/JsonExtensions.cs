using EquipTrack.Core.JsonResolvers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EquipTrack.Core.Extensions;

public static class JsonExtensions
{
    private static readonly Lazy<JsonSerializerOptions> LazyOptions =
        new(() => new JsonSerializerOptions().Configure(), isThreadSafe: true);

    /// <summary>
    /// Deserialize a JSON string into the specified type T using the configured options.
    /// Returns default(T) if the input is null, empty, or invalid JSON.
    /// </summary>
    public static T? FromJson<T>(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, LazyOptions.Value);
        }
        catch (JsonException)
        {
            // Optionally log or handle the exception here
            return default;
        }
    }

    /// <summary>
    /// Serialize an object to JSON using the configured options.
    /// Returns an empty string if the object is null or default.
    /// </summary>
    public static string ToJson<T>(this T value, bool indented = false)
    {
        if (value.IsDefault())
            return string.Empty;

        var options = indented
            ? new JsonSerializerOptions(LazyOptions.Value) { WriteIndented = true }
            : LazyOptions.Value;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Configure JsonSerializerOptions globally for the application.
    /// </summary>
    public static JsonSerializerOptions Configure(this JsonSerializerOptions settings)
    {
        settings.WriteIndented = false;
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        settings.ReadCommentHandling = JsonCommentHandling.Skip;
        settings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        settings.TypeInfoResolver = new PrivateConstructorContractResolver();
        settings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return settings;
    }
}