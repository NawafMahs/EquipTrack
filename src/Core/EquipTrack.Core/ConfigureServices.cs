using EquipTrack.Core.AppSettings;
using EquipTrack.Core.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace EquipTrack.Core;

[ExcludeFromCodeCoverage]
public static class ConfigureServices
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services) =>
           services
                   .AddOptionWithValidation<ConnectionOptions>()
                   .AddOptionWithValidation<CacheOptions>();



    /// <summary>
    /// Adds options with validation to the service collection.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to add.</typeparam>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddOptionWithValidation<TOptions>(this IServiceCollection services) where TOptions : class, IAppOptions
    {
        return services
                        .AddOptions<TOptions>()
                        .Configure<IConfiguration>((options, config) => 
                        {
                            config.GetSection(TOptions.ConfigSectionPath).Bind(options);
                        })
                        .ValidateDataAnnotations()
                        .ValidateOnStart()
                        .Services;
    }

}
