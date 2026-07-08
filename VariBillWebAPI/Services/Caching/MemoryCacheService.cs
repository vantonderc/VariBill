using Microsoft.Extensions.Caching.Memory;
using VariBillWebAPI.Services.Abstractions;

namespace VariBillWebAPI.Services.Caching;

/// <summary>
/// In‑memory cache service.
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiry.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiry.Value;
        else
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // For simplicity, we don't implement pattern removal with IMemoryCache.
        // In production, use Redis or iterate over keys if you have a list.
        return Task.CompletedTask;
    }
}