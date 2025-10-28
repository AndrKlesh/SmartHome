using AuthService.Jwt;
using AuthService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace AuthService.UnitTests;

internal sealed class LoginServiceTests
{
	[Test]
	public void LoginTest ()
	{
		IConfiguration conf = TestConfig.Build();
		LoginService ls = new(conf);
		string username = "user";
		(string accessToken, string refreshToken) = ls.Login(username, username);
		Assert.That(accessToken, Is.Not.EqualTo(string.Empty));
		bool isValid = ls.CheckToken(accessToken, out string usernameCheckToken);
		Assert.That(isValid, Is.True);
		Assert.That(usernameCheckToken, Is.EqualTo(username));
	}
}

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

internal static class TestConfig
{
	public static IConfiguration Build ()
	{
		Dictionary<string, string?> data = new()
		{
			["Jwt:Key"] = "F8h#9sLm@2vX!zP$QeRtY&5*KdNwUoG1",
			["users:0:login"] = "user",
			["users:0:passwordSha512"] = ComputeSha512Hex("user"),
			["users:0:role"] = "User",
		};
		return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
	}

	private static string ComputeSha512Hex (string input)
	{
		using var sha = System.Security.Cryptography.SHA512.Create();
		byte [] bytes = System.Text.Encoding.UTF8.GetBytes(input);
		byte [] hash = sha.ComputeHash(bytes);
		var sb = new System.Text.StringBuilder(hash.Length * 2);
		foreach (byte b in hash) sb.Append(b.ToString("x2"));
		return sb.ToString();
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
