using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using EquipTrack.Application.Common.Interfaces;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that caches query results.
/// Implements the caching pipeline pattern following NexusCore architecture.
/// Only applies to queries (read operations), not commands.
/// </summary>
/// <typeparam name="TRequest">The type of the request (query)</typeparam>
/// <typeparam name="TResponse">The type of the response (Result)</typeparam>
public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    // Cache configuration
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly MemoryCacheEntryOptions DefaultCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = DefaultCacheDuration,
        SlidingExpiration = TimeSpan.FromMinutes(2)
    };

    public CachingBehavior(
        IMemoryCache cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only cache queries, not commands
        if (!IsQuery(request))
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        var cacheKey = GenerateCacheKey(request);

        _logger.LogDebug("Checking cache for query {RequestName} with key {CacheKey}", 
            requestName, cacheKey);

        // Try to get from cache
        if (_cache.TryGetValue<TResponse>(cacheKey, out var cachedResponse) && cachedResponse != null)
        {
            _logger.LogDebug("Cache hit for query {RequestName}", requestName);
            return cachedResponse;
        }

        _logger.LogDebug("Cache miss for query {RequestName}, executing query", requestName);

        // Execute the query
        var response = await next();

        // Only cache successful results
        if (response.IsSuccess)
        {
            var cacheOptions = GetCacheOptions(request);
            _cache.Set(cacheKey, response, cacheOptions);
            
            _logger.LogDebug(
                "Cached result for query {RequestName} with expiration {Expiration}", 
                requestName, 
                cacheOptions.AbsoluteExpirationRelativeToNow);
        }
        else
        {
            _logger.LogDebug("Not caching failed result for query {RequestName}", requestName);
        }

        return response;
    }

    /// <summary>
    /// Determines if the request is a query (read operation).
    /// </summary>
    private static bool IsQuery(TRequest request)
    {
        // Check if request implements IQuery<TResponse>
        var requestType = request.GetType();
        var interfaces = requestType.GetInterfaces();
        
        return interfaces.Any(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == typeof(IQuery<>));
    }

    /// <summary>
    /// Generates a unique cache key for the request.
    /// </summary>
    private static string GenerateCacheKey(TRequest request)
    {
        var requestType = typeof(TRequest).Name;
        var requestHash = GetRequestHashCode(request);
        
        return $"{requestType}_{requestHash}";
    }

    /// <summary>
    /// Generates a hash code based on request properties.
    /// </summary>
    private static int GetRequestHashCode(TRequest request)
    {
        var hashCode = new HashCode();
        
        // Add all public properties to hash
        var properties = request.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(request);
            if (value != null)
            {
                hashCode.Add(value);
            }
        }
        
        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Gets cache options for the specific request type.
    /// Can be customized based on query attributes or conventions.
    /// </summary>
    private static MemoryCacheEntryOptions GetCacheOptions(TRequest request)
    {
        // Check if request has custom cache attribute (future enhancement)
        // For now, use default options
        
        // Different cache durations for different query types
        var requestName = typeof(TRequest).Name;
        
        return requestName switch
        {
            // Longer cache for relatively static data
            var name when name.Contains("GetAssetCategories") 
                => new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                },
            
            // Medium cache for semi-static data
            var name when name.Contains("GetAssets") || name.Contains("GetSpareParts")
                => new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                },
            
            // Short cache for dynamic data
            var name when name.Contains("GetMaintenanceTasks") || name.Contains("GetWorkOrders")
                => new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                },
            
            // Default
            _ => DefaultCacheOptions
        };
    }
}

/// <summary>
/// Attribute to mark queries that should not be cached.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class NoCacheAttribute : Attribute
{
}

/// <summary>
/// Attribute to specify custom cache duration for a query.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CacheDurationAttribute : Attribute
{
    public TimeSpan Duration { get; }

    public CacheDurationAttribute(int minutes)
    {
        Duration = TimeSpan.FromMinutes(minutes);
    }
}


