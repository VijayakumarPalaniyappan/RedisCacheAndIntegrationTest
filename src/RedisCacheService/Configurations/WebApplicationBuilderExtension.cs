using Azure.Identity;
using RedisCacheService.ConfigOptions;
using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.Configurations
{
  [ExcludeFromCodeCoverage]
  public static class WebApplicationBuilderExtension
  {
    public static void ConfigureAzureKeyVault(this WebApplicationBuilder builder, AzureConfig azConfig, KeyVaultConfig kvConfig)
    {
      builder.Configuration.AddAzureKeyVault(
          kvConfig.VaultUrl,
          new ClientSecretCredential(azConfig.TenantId, azConfig.ServicePrincipalId, kvConfig.ClientSecret));
    }
  }
}
