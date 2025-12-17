using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisCacheService.CustomExceptions;
using RedisCacheService.Filters;
using RedisCacheService.Services;

namespace RedisCacheService.Controllers
{
  [Route("user")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly ILogger<UserController> logger;
    private readonly IUserService userService;
    public UserController(
      ILogger<UserController> logger,
      IUserService userService
      )
    {
      this.logger = logger;
      this.userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserAsync()
    {
      try
      {
        logger.LogInformation("Request started to get user authorizations from BALDO.");

        if (string.IsNullOrEmpty(HttpContext.Request.Headers.Authorization))
        {
          return BadRequest("Authorization header is missing.");
        }
        var token = HttpContext.Request.Headers.Authorization;
        var oid = FilterOid.GetOid(token);

        if (oid == null)
        {
          logger.LogError("Invalid token!");
          return StatusCode(StatusCodes.Status401Unauthorized, "Invalid token!");
        }

        var userResponse = await userService.GetUserDetailstAsync(token, oid);
        if (userResponse == null)
        {
          return NotFound("User not found in Baldo.");
        }

        return Ok(userResponse);
      }
      catch (Exception ex)
      {
        return HandleException(ex, "Error getting user authorizations from BALDO.");
      }
    }

    private ObjectResult HandleException(Exception ex, string logMessage)
    {
      logger.LogError(ex, logMessage);

      return ex switch
      {
        HttpRequestException httpEx => StatusCode((int)httpEx.StatusCode, httpEx.Message),
        TaskCanceledException => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
        FormatException => StatusCode(StatusCodes.Status400BadRequest, ex.Message),
        JsonReaderException => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
        AuthCustomException => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
        _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.")
      };
    }
  }
}
