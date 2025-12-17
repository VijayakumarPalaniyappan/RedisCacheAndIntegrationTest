
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedisCacheService.ConfigOptions;
using RedisCacheService.CustomExceptions;
using RedisCacheService.Models;
using RedisCacheService.Services.AzureRedisCache;
using System.Net.Http.Headers;
using static Oneview.AuthService.RestApi.RedisServiceConstants;

namespace RedisCacheService.Services
{
  public class UserService : IUserService
  {
    private const string baldoApiKeyName = "x-gw-client-id";
    private const string baldoApiSecretKeyName = "x-gw-client-secret";
    private readonly HttpClient httpClient;
    private readonly ILogger<UserService> logger;
    private readonly BaldoOptions baldoOptions;
    private const string CacheKeyPrefixAuthorizations = "UserAuthorizations_";
    private readonly IRedisCache redisCache;

    public UserService(
      IHttpClientFactory httpClientFactory,
      ILogger<UserService> logger,
      IOptions<BaldoOptions> baldoOptions,
      IOptions<HttpClientSetting> httpClientSetting,
      IRedisCache redisCache)
    {
      this.logger = logger;
      this.baldoOptions = baldoOptions.Value;
      httpClient = httpClientFactory.CreateClient(httpClientSetting.Value.HttpClientName);
      this.redisCache = redisCache;
    }

    public async Task<User> GetUserDetailstAsync(string token, string oId)
    {
      User userResponse = null;

      // Check cache from Redis and return the details
      string cacheKey = CacheKeyPrefixAuthorizations + oId;
      var redisCacheResponse = redisCache.GetCacheData<User>(cacheKey);
      if (redisCacheResponse != null)
      {
        logger.LogInformation("User authorizations retrieved from cache.");
        return redisCacheResponse;
      }
      logger.LogInformation("The cache is empty!");
      var userCredentials = await GetUserCredentialsAsync(token);
      userResponse = MapUserCredentials(userCredentials);

      // Set 
      redisCache.SetCacheData(CacheKeyPrefixAuthorizations + oId, userResponse);

      logger.LogInformation("Request to get user authorizations from BALDO completed successfully.");
      return userResponse;
    }

    public async Task<UserCredential> GetUserCredentialsAsync(string token)
    {
      try
      {
        using HttpRequestMessage request = CreateHttpRequest(token, BaldoEndPoint.CurrentUserEndpoint);
        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string data = await response.Content.ReadAsStringAsync();
        var userCredential = JsonConvert.DeserializeObject<UserCredential>(data);
        logger.LogInformation("User credential received successfully.");

        return userCredential;
      }
      catch (HttpRequestException ex)
      {
        logger.LogError(ex, "Error getting user credentials from BALDO.");
        throw;
      }
      catch (TaskCanceledException ex)
      {
        logger.LogError(ex, "The request was cancelled while getting user credentials.");
        throw;
      }
      catch (FormatException ex)
      {
        logger.LogError(ex, "Invalid format exception while getting user credentials from Baldo.");
        throw;
      }
      catch (JsonReaderException ex)
      {
        logger.LogError(ex, "JsonReaderException while parsing user credentials response.");
        throw;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Internal server error occurred while getting user credentials from Baldo.");
        throw new AuthCustomException(ex.Message);
      }
    }

    private HttpRequestMessage CreateHttpRequest(string token, string endpoint)
    {
      var auth = AuthenticationHeaderValue.Parse(token);
      Uri.TryCreate(baldoOptions.ApiUrl + endpoint, UriKind.Absolute, out Uri baldoUri);
      HttpRequestMessage request = new()
      {
        RequestUri = baldoUri,
        Method = HttpMethod.Get,
        Headers =
        {
          { baldoApiKeyName, baldoOptions.ApiClientId },
          { baldoApiSecretKeyName, baldoOptions.ApiClientSecret }
        }
      };

      request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, auth.Parameter);

      return request;
    }

    public static User MapUserCredentials(UserCredential userCredential)
    {
      return new User
      {
        UserId = userCredential.UserId,
        FirstName = userCredential.FirstName,
        LastName = userCredential.LastName
      };
    }
  }
}
