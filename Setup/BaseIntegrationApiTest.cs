using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using RedisCacheService.ConfigOptions;
using RedisCacheService.Services;
using RedisCacheService.Services.AzureRedisCache;
using RedisCacheService.TestCommon.HttpContextConfig;
using RedisCacheService.TestCommon.HttpMock;
using RedisCacheService.TestCommon.MockData;
using StackExchange.Redis;

namespace RedisCacheService.IntegrationTests.Setup
{
  public class BaseIntegrationApiTest : IClassFixture<IntegrationTestFactory>
  {
    protected string accessToken;
    protected HttpClient httpClientWithAuthHeader;
    protected HttpClient httpClientWithoutAuthHeader;
    protected HttpMessageHandlerMock mockHttpMessageHandler;
    protected ILogger<UserService> loggerUserService;
    protected IOptions<RedisCacheOptions> redisCacheOptions;

    private IHttpClientFactory clientFactory;
    private IUserService userService;
    private ILogger<RedisCache> loggerRedis;
    private HttpClient httpClient;
    private IRedisCache redisCache;

    public BaseIntegrationApiTest(IntegrationTestFactory factory)
    {
      accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvaWQiOiJ1c2VyMSJ9.abc123";

      // Creates http client without configuring authentication schema
      httpClientWithoutAuthHeader = factory.CreateClient();

      clientFactory = Substitute.For<IHttpClientFactory>();
      loggerUserService = Substitute.For<ILogger<UserService>>();
      loggerRedis = Substitute.For<ILogger<RedisCache>>();

      mockHttpMessageHandler = Substitute.ForPartsOf<HttpMessageHandlerMock>();
      httpClient = new HttpClient(mockHttpMessageHandler, false);
      var connectionMultiplexer = factory.Services.CreateScope().ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

      // Should be flushed all database from ConnectionMultiplexer. Otherwise, the key will be reatined at database
      // It's very safe to flush the database to pass the subsequent integration test cases
      foreach (var server in connectionMultiplexer?.GetServers())
      {
        server?.FlushDatabase();
      }

      redisCacheOptions = Substitute.For<IOptions<RedisCacheOptions>>();
      redisCacheOptions.Value.Returns(new RedisCacheOptions() { CacheExpirationInHours = 0.00002 });
      clientFactory.CreateClient(BaldoMock.GetHttpClientSetting.Value.HttpClientName).Returns(httpClient);
      redisCache = new RedisCache(loggerRedis, connectionMultiplexer, redisCacheOptions);
      userService = new UserService(
        clientFactory,
        loggerUserService,
        BaldoMock.GetBaldoOptions,
        BaldoMock.GetHttpClientSetting,
        redisCache);

      // Creates http client with authentication schema configured
      httpClientWithAuthHeader = factory
        .WithService(_ => userService)
        .CreateAndConfigureClient(accessToken);
    }
  }
}
