using AuthService.Jwt;
using AuthService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace AuthService.UnitTests;

internal class JwtMiddlewareTests
{
	private IConfiguration _configuration;
	private LoginService _loginService;
	private DefaultHttpContext _httpContext;
	private JwtMiddleware _middleware;

	[SetUp]
	public void SetUp ()
	{
		_configuration = TestConfig.Build();
		_loginService = new LoginService(_configuration);
		_middleware = new JwtMiddleware(context => Task.CompletedTask, _loginService);
		_httpContext = new DefaultHttpContext();
	}

	[Test]
	public async Task InvokeValidTokenSetsAuthenticatedUser ()
	{
		string username = "user";
		(string token, string refresh) = _loginService.Login(username, username);
		_httpContext.Request.Path = "/SmartHome/dashboard/devices";
		Dictionary<string, string> cookies = new()
		{ { "jwt", token } };
		_httpContext.Request.Cookies = new MockCookieCollection(cookies);
		await _middleware.Invoke(_httpContext).ConfigureAwait(false);
		Assert.That(_httpContext.Items ["User"], Is.EqualTo("AuthenticatedUser"));
		Assert.That(_httpContext.Response.StatusCode, Is.Not.EqualTo(401));
	}

	[Test]
	public async Task InvokeInvalidTokenReturnsUnauthorized ()
	{
		_httpContext.Request.Path = "/SmartHome/dashboard/devices";
		Dictionary<string, string> cookies = new()
		{ { "jwt", "invalid-token" } };
		_httpContext.Request.Cookies = new MockCookieCollection(cookies);
		await _middleware.Invoke(_httpContext).ConfigureAwait(false);
		Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(401));
		Assert.That(_httpContext.Items ["User"], Is.Null);
	}

	[Test]
	public async Task InvokePublicPathSkipsTokenCheck ()
	{
		_httpContext.Request.Path = "/SmartHome/info";
		Dictionary<string, string> cookies = new();
		_httpContext.Request.Cookies = new MockCookieCollection(cookies);
		await _middleware.Invoke(_httpContext).ConfigureAwait(false);
		Assert.That(_httpContext.Response.StatusCode, Is.Not.EqualTo(401));
		Assert.That(_httpContext.Items ["User"], Is.Null);
	}
}
