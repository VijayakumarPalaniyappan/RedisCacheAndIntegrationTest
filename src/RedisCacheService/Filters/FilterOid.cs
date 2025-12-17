using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace RedisCacheService.Filters
{
  public class FilterOid
  {
    private static readonly JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

    public static string? GetOid(StringValues token)
    {
      string? oid = null;
      try
      {
        if (AuthenticationHeaderValue.TryParse(token, out var parsedAuthHeader))
        {
          var jsonToken = handler.ReadToken(parsedAuthHeader.Parameter) as JwtSecurityToken;
          oid = jsonToken?.Claims?.FirstOrDefault(claim => claim.Type.Equals("oid"))?.Value;
        }
      }
      catch (Exception)
      {
        return null;
      }

      return oid;
    }
  }
}
