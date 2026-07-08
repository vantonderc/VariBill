namespace VariBillWebAPI.Services.Abstractions;

/// <summary>
/// Cache service abstraction used for short-lived data caching.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Stores a value in cache with optional expiry.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// Removes a cached entry by key.
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes cached entries matching the given pattern.
    /// </summary>
    Task RemoveByPatternAsync(string pattern);
}



//namespace VariBillWebAPI.Services.Interfaces
//{
//    public interface ICacheService
//    {
//        Task<T?> GetAsync<T>(string key);
//        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
//        Task RemoveAsync(string key);
//        Task RemoveByPatternAsync(string pattern);
//    }
//}
