using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.Models
{
  public class UserCredential
  {
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [SuppressMessage("SonarLint", "S4004", Justification = "Ignored intentionally as it is part of DTO")]
    public List<string> UserOrganizations { get; set; }
  }
}
