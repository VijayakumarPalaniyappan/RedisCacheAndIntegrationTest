using RedisCacheService.ConfigOptions;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.Configurations
{
  [ExcludeFromCodeCoverage]
  [SuppressMessage("SonarLint", "S4462", Justification = "No need of asynchronous call")]
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddRedis(this IServiceCollection service, AzureConfig azureConfig, string host, bool isDevelopment)
    {
      service.AddSingleton<IConnectionMultiplexer>(sp =>
      {
        if (isDevelopment)
        {
          var options = ConfigurationOptions.Parse(host);
          options.Ssl = true;

          return ConnectionMultiplexer.Connect(options);
        }

        var configurationOptions = ConfigurationOptions.Parse(host)
                                    .ConfigureForAzureWithServicePrincipalAsync(azureConfig.ServicePrincipalId, azureConfig.TenantId,
                                    azureConfig.ClientSecret).Result;
        configurationOptions.Ssl = true;

        return ConnectionMultiplexer.Connect(configurationOptions);
      });
      return service;
    }
  }
}
