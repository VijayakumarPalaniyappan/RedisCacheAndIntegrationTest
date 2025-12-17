using RedisCacheService.ConfigOptions;
using RedisCacheService.Configurations;
using RedisCacheService.Middlewares;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService
{
  [ExcludeFromCodeCoverage]
  [SuppressMessage("SonarLint", "S1118", Justification = "Should be public")]
  [SuppressMessage("SonarLint", "S4792", Justification = "Logs stored in a secure location")]
  public class Program
  {
    public static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console().CreateLogger();

      try
      {
        Log.Information("Starting up the service.");
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

        var azureConfig = builder.Configuration.GetSection("AzureAdSettings").Get<AzureConfig>();
        if (builder.Environment.IsProduction())
        {
          var kvConfig = builder.Configuration.GetSection("AzureKeyVault").Get<KeyVaultConfig>();
          builder.ConfigureAzureKeyVault(azureConfig, kvConfig);
        }

        azureConfig = builder.Configuration.GetSection("AzureAdSettings").Get<AzureConfig>();

        // Add services to the container.
        var redisCacheOption = builder.Configuration.GetSection("RedisCacheOptions");
        builder.Services.Configure<BaldoOptions>(builder.Configuration.GetSection("BaldoOptions"));

        // Caching - InMemory
        var redisHostname = redisCacheOption.GetSection("HostName").Value;
        builder.Services.AddRedis(azureConfig, redisHostname, builder.Environment.IsDevelopment());

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
          app.UseSwagger();
          app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<SerilogRequestMiddleware>();
        app.UseSerilogRequestLogging();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Service terminated unexpectedly.");
      }
      finally
      {
        Log.CloseAndFlush();
      }

    }
  }
}