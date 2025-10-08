using EquipTrack.Core.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EquipTrack.Core.Extensions;

/// <summary>
/// Provides extension methods for retrieving configuration options from the application's service provider.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Retrieves the registered <typeparamref name="TOptions"/> instance from the service provider.
    /// Ensures that the options have been properly registered and bound to configuration.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options to retrieve. Must implement <see cref="IAppOptions"/>.</typeparam>
    /// <param name="serviceProvider">The application's service provider.</param>
    /// <returns>The resolved options instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the options are not registered in the DI container.</exception>
    public static TOptions GetRequiredOptions<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class, IAppOptions
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var options = serviceProvider.GetService<IOptions<TOptions>>()?.Value;
        if (options is null)
        {
            var typeName = typeof(TOptions).Name;
            throw new InvalidOperationException(
                $"Options of type '{typeName}' are not registered. " +
                "Ensure services.AddOptions is called with the correct configuration section.");
        }

        return options;
    }
}