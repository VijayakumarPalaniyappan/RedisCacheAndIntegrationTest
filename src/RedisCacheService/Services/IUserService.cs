using RedisCacheService.Models;

namespace RedisCacheService.Services
{
  public interface IUserService
  {
    Task<User> GetUserDetailstAsync(string token, string oId);
  }
}
