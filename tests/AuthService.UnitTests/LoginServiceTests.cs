using AuthService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
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
		bool isValid = ls.CheckToken(token);
		Assert.That(isValid, Is.True);
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
