using Microsoft.Extensions.Options;
using RedisCacheService.ConfigOptions;

namespace RedisCacheService.TestCommon.MockData
{
  public class RedisCacheMock
  {
    public static IOptions<RedisCacheOptions> GetRedisCacheOptions => Options.Create(new RedisCacheOptions
    {
      CacheExpirationInHours = 8
    });
  }
}
