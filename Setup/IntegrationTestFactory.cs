using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace RedisCacheService.IntegrationTests.Setup
{
  public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
  {
    private RedisContainer redisContainer = new RedisBuilder().WithImage("redis:latest").WithPortBinding(6379, true).Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureAppConfiguration((context, config) =>
      {
        string configPath = Path.Combine(Directory.GetParent(
          Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Setup", "appsettings.json");
        config.AddJsonFile(configPath, optional: false);
      });
      base.ConfigureWebHost(builder);
      builder.ConfigureTestServices(service =>
      {
        service.RemoveAll<IConnectionMultiplexer>();
        service.AddSingleton<IConnectionMultiplexer>(sp =>
        {
          var options = ConfigurationOptions.Parse(redisContainer.GetConnectionString());
          // Before flushing all caching server, it should have admin access. Stated clearly: BaseIntegrationApiTest class
          options.AllowAdmin = true;

          return ConnectionMultiplexer.Connect(options);
        });
      });
    }

    // Called once before any tests run.
    public async Task InitializeAsync()
    {
      // Start the container.
      await redisContainer.StartAsync();
    }

    // Called once after all tests have finished.
    async Task IAsyncLifetime.DisposeAsync()
    {
      // Stop and dispose of the container.      
      await redisContainer.DisposeAsync();
    }
  }
}
