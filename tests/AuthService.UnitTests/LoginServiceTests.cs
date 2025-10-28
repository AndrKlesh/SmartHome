using AuthService.Services;
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
