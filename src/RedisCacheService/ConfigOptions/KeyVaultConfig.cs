using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.ConfigOptions
{
  [ExcludeFromCodeCoverage]
  public class KeyVaultConfig
  {
    public Uri VaultUrl { get; set; }
    public string ClientSecret { get; set; }
  }
}
