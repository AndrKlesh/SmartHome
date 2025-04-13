using AuthService.Jwt;
using AuthService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace AuthService.UnitTests;

internal sealed class LoginServiceTests
{
	[Test]
	public void LoginTest ()
	{
		ConfStub conf = new();
		LoginService ls = new(conf);
		string username = "user";
		string token = ls.Login(username, username);
		Assert.That(token, Is.Not.EqualTo(string.Empty));
		bool isValid = ls.CheckToken(token, out string usernameCheckToken);
		Assert.That(isValid, Is.True);
		Assert.That(isValid, username);
	}
}

internal class JwtMiddlewareTests
{
	private Mock<IConfiguration> _configMock;
	private LoginService _loginService;
	private DefaultHttpContext _httpContext;
	private JwtMiddleware _middleware;

	[SetUp]
	public void SetUp ()
	{
		_configMock = new Mock<IConfiguration>();
		_ = _configMock.Setup(c => c ["Jwt:Key"]).Returns("F8h#9sLm@2vX!zP$QeRtY&5*KdNwUoG1");
		_loginService = new LoginService(_configMock.Object);
		_middleware = new JwtMiddleware(context => Task.CompletedTask, _loginService);
		_httpContext = new DefaultHttpContext();
	}

	[Test]
	public async Task InvokeValidTokenSetsAuthenticatedUser ()
	{
		string username = "user";
		string token = _loginService.Login(username, username);
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

internal sealed class ConfStub : IConfiguration
{
	public string? this [string key] { get => "F8h#9sLm@2vX!zP$QeRtY&5*KdNwUoG1"; set => throw new NotImplementedException(); }

	public IEnumerable<IConfigurationSection> GetChildren ()
	{
		throw new NotImplementedException();
	}

	public IChangeToken GetReloadToken ()
	{
		throw new NotImplementedException();
	}

	public IConfigurationSection GetSection (string key)
	{
		throw new NotImplementedException();
	}
}

internal class MockCookieCollection (Dictionary<string, string> cookies) : IRequestCookieCollection
{
	private readonly Dictionary<string, string> _cookies = cookies;

	public string this [string key] => _cookies.TryGetValue(key, out var value) ? value : null;

	public int Count => _cookies.Count;

	public ICollection<string> Keys => _cookies.Keys;

	public bool ContainsKey (string key)
	{
		return _cookies.ContainsKey(key);
	}

	public IEnumerator<KeyValuePair<string, string>> GetEnumerator ()
	{
		return _cookies.GetEnumerator();
	}

	public bool TryGetValue (string key, out string value)
	{
		return _cookies.TryGetValue(key, out value);
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	{
		return GetEnumerator();
	}
}
