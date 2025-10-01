namespace EquipTrack.Domain.Common;

/// <summary>
/// Helper class for enforcing domain invariants.
/// Keeps validation logic consistent and reusable across entities.
/// </summary>
public static class Ensure
{
    public static string NotEmpty(string value, string paramName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{paramName} cannot be empty.", paramName)
            : value;

    public static T NotNull<T>(T value, string paramName) where T : class =>
        value ?? throw new ArgumentNullException(paramName);

    public static decimal Positive(decimal value, string paramName) =>
        value <= 0
            ? throw new ArgumentException($"{paramName} must be positive.", paramName)
            : value;

    public static int Positive(int value, string paramName) =>
        value <= 0
            ? throw new ArgumentException($"{paramName} must be positive.", paramName)
            : value;

    public static decimal NotNegative(decimal value, string paramName) =>
        value < 0
            ? throw new ArgumentException($"{paramName} cannot be negative.", paramName)
            : value;

    public static Guid NotEmpty(Guid value, string paramName) =>
        value == Guid.Empty
            ? throw new ArgumentException($"{paramName} cannot be empty.", paramName)
            : value;

    public static DateTime NotDefault(DateTime value, string paramName) =>
        value == default
            ? throw new ArgumentException($"{paramName} cannot be default.", paramName)
            : value;
}
