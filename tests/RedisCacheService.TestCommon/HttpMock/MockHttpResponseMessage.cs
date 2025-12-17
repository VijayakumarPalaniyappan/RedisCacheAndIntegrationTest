using System.Net;
using System.Text;

namespace RedisCacheService.TestCommon.HttpMock
{
  public class MockHttpResponseMessage
  {
    public static HttpResponseMessage CreateHttpResponseMessage(
      string userResponseJson,
      HttpStatusCode statusCode = HttpStatusCode.OK)
    {
      return new HttpResponseMessage
      {
        StatusCode = statusCode,
        Content = new StringContent(userResponseJson, Encoding.UTF8, "application/json"),
      };
    }
  }
}
