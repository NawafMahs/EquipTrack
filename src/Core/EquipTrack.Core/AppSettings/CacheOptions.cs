using EquipTrack.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace EquipTrack.Core.AppSettings;

/// <summary>
/// Represents the caching configuration settings used by the application.
/// </summary>
/// <remarks>
/// This class defines the expiration policies for cached data within the application.
/// It provides options for both absolute and sliding expiration, allowing fine-grained control 
/// over cache lifetime behavior.  
/// 
/// It implements <see cref="IAppOptions"/> to support automatic binding from the configuration section 
/// named after this class (<c>"CachOptions"</c>).
/// </remarks>
public sealed class CacheOptions : IAppOptions
{
    /// <inheritdoc/>
    static string IAppOptions.ConfigSectionPath => nameof(CacheOptions);

    /// <summary>
    /// Gets the number of hours before a cache entry absolutely expires and is removed from the cache.
    /// </summary>
    /// <remarks>
    /// Use this setting to define a fixed lifetime for cached data, regardless of access frequency.  
    /// A typical value might range between <c>1</c> and <c>24</c> hours depending on data volatility.
    /// </remarks>
    /// <example>
    /// If <see cref="AbsoluteExpirationInHours"/> is set to <c>6</c>, all cache entries will be removed 
    /// exactly six hours after creation.
    /// </example>
    [Range(1, 24)] // 1 hour to 24 
    public int AbsoluteExpirationInHours { get; private init; }

    /// <summary>
    /// Gets the sliding expiration duration in seconds for cache entries.
    /// </summary>
    /// <remarks>
    /// Sliding expiration resets the cache entry’s lifetime on each access.  
    /// This helps keep frequently used items in cache longer while removing infrequently accessed items sooner.
    /// </remarks>
    /// <example>
    /// If <see cref="SlidingExpirationInSeconds"/> is <c>300</c>, the cache entry will expire 
    /// five minutes after the last access, unless accessed again within that time.
    /// </example>
    [Range(60, 3600)] // 1 minute to 1 hour
    public int SlidingExpirationInSeconds { get; private init; }
}
