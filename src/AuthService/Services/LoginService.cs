#pragma warning disable CA1515

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public sealed class LoginService (IConfiguration configuration)
{
	public string Login (string username, string password)
	{
		if (username == "user" && password == "user")
		{
			// Генерация JWT-токена
			string token = GenerateJwtToken(username);

			return token;
		}
		else
		{
			throw new UnauthorizedAccessException();
		}
	}

	private string GenerateJwtToken (string username)
	{
		byte [] key = Encoding.UTF8.GetBytes(configuration ["Jwt:Key"]);

		JwtSecurityTokenHandler tokenHandler = new();

		SecurityTokenDescriptor tokenDescriptor = new()
		{
			Subject = new ClaimsIdentity(new [] { new Claim(ClaimTypes.Name, username) }),
			Expires = DateTime.UtcNow.AddHours(1),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
		string tokenString = tokenHandler.WriteToken(token);
		return tokenString;
	}
}
