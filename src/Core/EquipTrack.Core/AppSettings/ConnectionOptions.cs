using EquipTrack.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace EquipTrack.Core.AppSettings;

/// <summary>
/// Represents the strongly-typed configuration settings for application connection strings.
/// </summary>
/// <remarks>
/// This class defines the connection endpoints used by the application to connect to various data sources,
/// including SQL databases, NoSQL stores, and caching systems. 
/// 
/// It implements <see cref="IAppOptions"/> to support automatic binding from the configuration section 
/// specified by <see cref="IAppOptions.ConfigSectionPath"/>.
/// </remarks>
public sealed class ConnectionOptions : IAppOptions
{
    /// <inheritdoc/>
    static string IAppOptions.ConfigSectionPath => "ConnectionStrings";

    /// <summary>
    /// Gets the connection string for the primary SQL database.
    /// </summary>
    /// <remarks>
    /// This connection is typically used for relational data access via Entity Framework Core or 
    /// ADO.NET-based repositories.
    /// </remarks>
    [Required]
    public string SqlConnection { get; private init; } = default!;

    /// <summary>
    /// Gets the connection string for the NoSQL data store.
    /// </summary>
    /// <remarks>
    /// Used to connect to document-oriented or key-value databases such as MongoDB, Couchbase, or similar.
    /// </remarks>
    [Required]
    public string NoSqlConnection { get; private init; } = default!;

    /// <summary>
    /// Gets the connection configuration for the caching layer.
    /// </summary>
    /// <remarks>
    /// This may represent an in-memory cache, a distributed cache such as Redis, or another caching provider.
    /// </remarks>
    [Required]
    public string CachConnection { get; private init; } = default!;

    /// <summary>
    /// Determines whether the configured cache connection is set to use an in-memory cache provider.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="CachConnection"/> value equals <c>"InMemory"</c> 
    /// (case-insensitive); otherwise, <see langword="false"/>.
    /// </returns>
    /// <example>
    /// The following example checks if the application should use in-memory caching:
    /// <code>
    /// if (options.CacheConnectionInMemory())
    /// {
    ///     services.AddMemoryCache();
    /// }
    /// else
    /// {
    ///     services.AddStackExchangeRedisCache(options =>
    ///         options.Configuration = connectionOptions.CachConnection);
    /// }
    /// </code>
    /// </example>
    public bool CacheConnectionInMemory() =>
        CachConnection.Equals("InMemory", StringComparison.InvariantCultureIgnoreCase);
}
