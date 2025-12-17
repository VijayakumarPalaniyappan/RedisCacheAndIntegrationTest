using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using RedisCacheService.IntegrationTests.Setup;
using RedisCacheService.TestCommon.HttpMock;
using RedisCacheService.TestCommon.MockData;

namespace RedisCacheService.IntegrationTests.RestApi
{
  public class RedisCacheServiceTest : BaseIntegrationApiTest
  {
    private readonly string authEndpoint = "/user/authorizations";
    private IntegrationTestFactory factory;

    public RedisCacheServiceTest(IntegrationTestFactory factory) : base(factory)
    {
      this.factory = factory;
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_AuthorizationHeaderMissing_ReturnsUnauthorizedResponse()
    {
      // Act
      var response = await httpClientWithoutAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
      Assert.Equal("Authorization header is missing.", data);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetUserAuthorizationsAsync_AuthorizationTokenMissing_ReturnsUnauthorizedResponse(string invalidToken)
    {
      // Act
      accessToken = invalidToken;
      var response = await httpClientWithoutAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
      Assert.Equal("Authorization header is missing.", data);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_InvalidJsonStringResponse_ReturnsInternalServerErrorResponse()
    {
      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
      Assert.Equal("Internal server error.", data);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_UserCredentialsWithEmptyUserOrganizations_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponseWithEmptyUserOrganizations();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(mockUserCredentialsResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Null(user.PrincipalOrganizationId);
      Assert.Null(user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_NoAuthorizedApplication_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationResponseWithNoApplications();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("User unauthorized", HttpStatusCode.Unauthorized)]
    [InlineData("Bad request format", HttpStatusCode.BadRequest)]
    public async Task GetUserAuthorizationsAsync_UserCredentialsIsNull_ReturnsNotFoundResponse(string httpResponse, HttpStatusCode statusCode)
    {
      // Arrange
      var errMessage = $"Response status code does not indicate success: {(int)statusCode}";
      var userResponseJson = BaldoMock.CreateCurrentUserResponseWithEmptyUserOrganizations();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(httpResponse, statusCode);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(mockUserCredentialsResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      Assert.Contains(errMessage, data);
      Assert.Equal(statusCode, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_OneviewNotInAuthorizedApplication_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationResponseWithNoOneviewInApplicationsList();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<ushort>(data);
      Assert.NotNull(user);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationResponse();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_UserAuthorizedWorkshop_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationWithWorkshopResponse();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_UserAuthorizedMainDealer_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationWithMainDealerResponse();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_UserAuthorizedMarket_ReturnsOkResponse()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationWithMarketResponse();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_NoAuthorizedApplication_ReturnsOkResponseFromCache()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationResponseWithNoApplications();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);
      // Second call for reading data from cache
      response = await httpClientWithAuthHeader.GetAsync(authEndpoint);
      Assert.NotNull(response);
      loggerUserService.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(),
        Arg.Is<object>(m => m.ToString().Contains("User authorizations retrieved from cache.")), null,
        Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task GetUserAuthorizationsAsync_CachedItemShouldExpire_AfterSpecifiedDuration()
    {
      // Arrange
      var userResponseJson = BaldoMock.CreateCurrentUserResponse();
      var mockUserCredentialsResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userResponseJson);

      string orgId = "1234XXYY";
      string orgName = "VTC Retail";
      var userOrgResponseJson = BaldoMock.CreateUserOrganizationResponse();
      var mockUserPrincipalOrgResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userOrgResponseJson);

      var userAuthResponseJson = BaldoMock.CreateAuthorizationResponseWithNoApplications();
      var mockUserAuthResponse = MockHttpResponseMessage.CreateHttpResponseMessage(userAuthResponseJson);

      mockHttpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .Returns(
        x => mockUserCredentialsResponse,
        x => mockUserPrincipalOrgResponse,
        x => mockUserAuthResponse);

      // Act
      var response = await httpClientWithAuthHeader.GetAsync(authEndpoint);

      // Assert
      Assert.NotNull(response);
      var data = await response.Content.ReadAsStringAsync();
      var user = JsonConvert.DeserializeObject<User>(data);
      Assert.NotNull(user);
      Assert.Equal(orgId, user.PrincipalOrganizationId);
      Assert.Equal(orgName, user.OrganizationName);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      loggerUserService.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(),
        Arg.Is<object>(m => m.ToString().Contains("The cache is empty!")), null,
        Arg.Any<Func<object, Exception, string>>());

      await Task.Delay(TimeSpan.FromSeconds(2)); // Wait for delay 2 seconds

      // Second call for reading data from cache
      response = await httpClientWithAuthHeader.GetAsync(authEndpoint);
      Assert.NotNull(response);
      loggerUserService.Received(2).Log(LogLevel.Information, Arg.Any<EventId>(),
        Arg.Is<object>(m => m.ToString().Contains("The cache is empty!")), null,
        Arg.Any<Func<object, Exception, string>>());
    }
  }
}
