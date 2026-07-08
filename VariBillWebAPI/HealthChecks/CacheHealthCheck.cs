using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VariBillWebAPI.HealthChecks;

/// <summary>
/// Health check for cache.
/// </summary>
public class CacheHealthCheck : IHealthCheck
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of <see cref="CacheHealthCheck"/>.
    /// </summary>
    /// <param name="cache">The memory cache to verify.</param>
    public CacheHealthCheck(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple test: set and retrieve a key
            _cache.Set("health_check", "ok");
            var result = _cache.Get<string>("health_check");
            return Task.FromResult(
                result == "ok"
                    ? HealthCheckResult.Healthy("Cache is working.")
                    : HealthCheckResult.Unhealthy("Cache is not working."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Cache is unreachable.", ex));
        }
    }
}
