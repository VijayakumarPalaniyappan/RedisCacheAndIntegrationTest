using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedisCacheService.ConfigOptions;
using StackExchange.Redis;

namespace RedisCacheService.Services.AzureRedisCache
{
  /// <summary>
  /// Redis cache
  /// </summary>
  public class RedisCache : IRedisCache
  {
    private readonly IDatabase? db;
    private readonly ILogger<RedisCache> logger;
    private readonly RedisCacheOptions redisCacheOptions;

    /// <summary>
    /// Constructor
    /// </summary>
    public RedisCache(ILogger<RedisCache> logger,
      IConnectionMultiplexer connectionMultiplexer,
      IOptions<RedisCacheOptions> redisCacheOptions)
    {
      this.logger = logger;
      db = connectionMultiplexer?.GetDatabase();
      this.redisCacheOptions = redisCacheOptions.Value;
    }

    /// <summary>
    /// Get cache data
    /// </summary>
    /// <typeparam name="T">type of entity class</typeparam>
    /// <param name="key">cache key</param>
    /// <returns>return type cache data using key</returns>
    public T? GetCacheData<T>(string key)
    {
      logger.LogInformation("Redis Cache Key value: {Key}", key);
      if (db is null)
      {
        logger.LogWarning("Redis Cache Database is null");
        return default;
      }

      var value = db.StringGet(key);
      if (string.IsNullOrEmpty(value))
      {
        return default;
      }
      return JsonConvert.DeserializeObject<T>(value.ToString());
    }

    /// <summary>
    /// Set cache data using key, type of object and expiration time
    /// </summary>
    /// <param name="key">cache key</param>
    /// <typeparam name="T">type of entity class</typeparam>
    /// <returns>return status of set cache</returns>
    public bool SetCacheData<T>(string key, T value)
    {
      if (db is null)
      {
        logger.LogWarning("Redis Cache Database is null and cannot set the data.");
        return false;
      }

      var expirationTime = TimeSpan.FromHours(redisCacheOptions.CacheExpirationInHours);
      var isSet = db.StringSet(key, JsonConvert.SerializeObject(value), expirationTime);
      logger.LogInformation("Successfully set Redis Cache using Key value: {Key} for setting up and status is: {isSet}", key, isSet);
      return isSet;
    }
  }
}
