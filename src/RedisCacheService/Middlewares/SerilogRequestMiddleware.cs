using RedisCacheService.Filters;
using Serilog.Context;
using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.Middlewares
{
  [ExcludeFromCodeCoverage]
  public class SerilogRequestMiddleware
  {
    private readonly RequestDelegate next;

    public SerilogRequestMiddleware(RequestDelegate next)
    {
      this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      using (LogContext.PushProperty("ClientId", FilterOid.GetOid(context.Request.Headers.Authorization)))
      using (LogContext.PushProperty("ClientRequestId", Guid.NewGuid().ToString()))
      {
        await next(context);
      }
    }
  }
}
