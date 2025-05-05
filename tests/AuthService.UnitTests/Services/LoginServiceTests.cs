using AuthService.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace AuthService.UnitTests.Services;

[TestFixture]
internal sealed class LoginServiceTests
{
	private Mock<IConfiguration> _configurationMock;
	private LoginService _loginService;

	[SetUp]
	public void Setup ()
	{
		_configurationMock = new Mock<IConfiguration>();
		_ = _configurationMock.Setup(cfg => cfg ["Jwt:Key"]).Returns("supersecretkey1234567890_superlongkey!");

		_loginService = new LoginService(_configurationMock.Object);
	}

	[Test]
	public void Login_ValidCredentials_ReturnsJwtToken ()
	{
		string token = _loginService.Login("user", "user");

		Assert.Multiple(() =>
		{
			Assert.That(token, Is.Not.Null.Or.Empty, "Token should not be null or empty");
			Assert.That(token.Split('.'), Has.Length.EqualTo(3), "Token should be in JWT format");
		});
	}

	[Test]
	public void Login_InvalidCredentials_ThrowsUnauthorizedAccessException ()
	{
		_ = Assert.Throws<UnauthorizedAccessException>(() => _loginService.Login("admin", "wrongpass"));
	}
}
