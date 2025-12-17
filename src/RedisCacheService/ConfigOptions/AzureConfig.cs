using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.ConfigOptions
{
  [ExcludeFromCodeCoverage]
  public class AzureConfig
  {
    public string TenantId { get; set; }
    public string ServicePrincipalId { get; set; }
    public string ClientSecret { get; set; }
  }
}
