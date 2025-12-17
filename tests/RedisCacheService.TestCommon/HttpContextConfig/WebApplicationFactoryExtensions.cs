using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace RedisCacheService.TestCommon.HttpContextConfig
{
  public static class WebApplicationFactoryExtensions
  {
    public static WebApplicationFactory<TFactory> WithService<TFactory, TService>(
      this WebApplicationFactory<TFactory> factory,
      Func<IServiceProvider, TService> implementationFactory)
      where TService : class
      where TFactory : class
    {
      return factory.WithWebHostBuilder(builder =>
      {
        builder.ConfigureTestServices(services =>
        {
          services.AddScoped(implementationFactory);
        });
      });
    }

    public static HttpClient CreateAndConfigureClient<T>(
      this WebApplicationFactory<T> factory, string accessToken) where T : class
    {
      var client = factory.CreateClient(new WebApplicationFactoryClientOptions
      {
        AllowAutoRedirect = false
      });

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      return client;
    }
  }
}
