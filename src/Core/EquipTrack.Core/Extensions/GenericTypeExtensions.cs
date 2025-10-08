namespace EquipTrack.Core.Extensions;

using System;
using System.Linq;

public static class GenericTypeExtensions
{
    /// <summary>
    /// Determines whether the specified value equals its type's default value.
    /// Works for both reference and value types.
    /// </summary>
    public static bool IsDefault<T>(this T value)
        => EqualityComparer<T>.Default.Equals(value, default!);

    /// <summary>
    /// Returns a human-readable generic type name for the specified object.
    /// Example: Dictionary&lt;String, List&lt;Int32&gt;&gt;
    /// </summary>
    public static string GetGenericTypeName(this object? obj)
    {
        if (obj is null)
            return "null";

        return GetFriendlyTypeName(obj.GetType());
    }

    private static string GetFriendlyTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var genericArguments = type.GetGenericArguments()
                                   .Select(GetFriendlyTypeName)
                                   .ToArray();

        var typeName = type.Name[..type.Name.IndexOf('`')];
        return $"{typeName}<{string.Join(", ", genericArguments)}>";
    }
}
