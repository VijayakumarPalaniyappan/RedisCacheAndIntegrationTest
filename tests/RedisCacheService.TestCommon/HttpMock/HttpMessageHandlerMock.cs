using System.Net;
using System.Text;

namespace RedisCacheService.TestCommon.HttpMock
{
  public class HttpMessageHandlerMock : HttpMessageHandler
  {
    private readonly TaskCanceledException taskCanceledException;
    private readonly HttpRequestException httpRequestException;
    private readonly FormatException formatException;

    public string RequestContent { get; private set; }
    public int NumberOfSendAsyncCalls { get; private set; }

    public HttpMessageHandlerMock() { }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      NumberOfSendAsyncCalls++;
      return await MockSendAsync(request, cancellationToken);
    }

    public virtual async Task<HttpResponseMessage> MockSendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      if (taskCanceledException != null)
      {
        throw taskCanceledException;
      }
      if (httpRequestException != null)
      {
        throw httpRequestException;
      }

      if (request.Content != null)
      {
        RequestContent = await request.Content.ReadAsStringAsync();
      }

      return new HttpResponseMessage
      {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent("Success", Encoding.UTF8, "application/json"),
      };
    }
  }
}
