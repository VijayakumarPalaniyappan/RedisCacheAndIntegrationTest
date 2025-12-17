namespace RedisCacheService.Services.AzureRedisCache
{
  public interface IRedisCache
  {
    /// <summary>
    /// Get cache data
    /// </summary>
    /// <typeparam name="T">type of entity class</typeparam>
    /// <param name="key">cache key</param>
    /// <returns>return type cache data using key</returns>
    T? GetCacheData<T>(string key);
    /// <summary>
    /// Set cache data using key, type of object and expiration time
    /// </summary>
    /// <param name="key">cache key</param>
    /// <typeparam name="T">type of entity class</typeparam>
    /// <returns>return status of set cache</returns>
    bool SetCacheData<T>(string key, T value);
  }
}
