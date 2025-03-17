using AuthService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;

namespace AuthService.UnitTests;

internal class LoginServiceTests
{
	[Test]
	public void LoginTest ()
	{
		ConfStub conf = new();
		LoginService ls = new(conf);
		string username = "user";
		string token = ls.Login(username, username);
		Assert.That(token, Is.Not.EqualTo(string.Empty));
	}
}

internal class ConfStub : IConfiguration
{
	public string? this [string key] { get => "qwertyuiopasdfghjklzxcvbnmqwertyu"; set => throw new NotImplementedException(); }

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
