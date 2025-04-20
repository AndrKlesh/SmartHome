#pragma warning disable CA1515
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public sealed class LoginService (IConfiguration configuration)
{
	private readonly Dictionary<string, string> _users = new()
	{
		{ "user", BCrypt.Net.BCrypt.HashPassword("user") } // Хэшированный пароль
    };
	public string Login (string username, string password)
	{
		if (_users.TryGetValue(username, out string hashedPassword) &&
			BCrypt.Net.BCrypt.Verify(password, hashedPassword))
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

	/*public bool Register (string username, string password)
	{
		if (_users.ContainsKey(username))
		{
			return false; // Пользователь уже существует
		}

		_users.Add(username, BCrypt.Net.BCrypt.HashPassword(password));
		return true;
	}*/

	private string GenerateJwtToken (string username)
	{
		byte [] key = Encoding.UTF8.GetBytes(configuration ["Jwt:Key"]);

		JwtSecurityTokenHandler tokenHandler = new();

		SecurityTokenDescriptor tokenDescriptor = new()
		{
			Subject = new ClaimsIdentity(new [] { new Claim(ClaimTypes.Name, username) }),
			Audience = "AUDIENCE",
			Issuer = "ISSUER",
			Expires = DateTime.UtcNow.AddHours(1),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
		string tokenString = tokenHandler.WriteToken(token);
		return tokenString;
	}

	public bool CheckToken (string token, out string username)
	{
		try
		{
			byte [] key = Encoding.UTF8.GetBytes(configuration ["Jwt:Key"]);
			JwtSecurityTokenHandler tokenHandler = new();
			TokenValidationParameters validationParameters = new()
			{
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidAudience = "AUDIENCE",
				ValidIssuer = "ISSUER",
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero
			};

			ClaimsPrincipal claims = tokenHandler.ValidateToken(token, validationParameters, out _);
			Claim userClaim = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
			username = userClaim.Value;
			return true;
		}
		catch (Exception ex)
		{
			username = string.Empty;
			return false;
		}
	}
}
