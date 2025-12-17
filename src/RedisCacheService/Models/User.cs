namespace RedisCacheService.Models
{
  public class User
  {
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PrincipalOrganizationId { get; set; }
    public string OrganizationName { get; set; }
  }
}
