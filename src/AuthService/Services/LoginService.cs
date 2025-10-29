#pragma warning disable CA1515
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AuthService.Services;

public sealed class LoginService (IConfiguration configuration)
{
	//TODO: Можно не создавать Dictionary, а просто обращаться к configuration.GetSection("users") каждый раз,
	//когда пользователь выполняет login
	//При этом можно сделать reloadOnChange = true для appsettings.json
	//В Program.cs добавить
	//builder.Configuration.AddJsonFile("appsettings.json", 
	//                                  optional: false, 
    //                                  reloadOnChange: true);

	private readonly Dictionary<string, (string PasswordSha512, string Role)> _users = BuildUsers(configuration);

	public (string AccessToken, string RefreshToken) Login (string username, string password)
	{
		//TODO: заменить _users.TryGetValue на функцию получения пользователя и его пароля из секции конфигурации
		if (_users.TryGetValue(username, out (string PasswordSha512, string Role) userInfo) &&
			string.Equals(ComputeSha512Hex(password), userInfo.PasswordSha512, StringComparison.OrdinalIgnoreCase))
		{
			// Генерация JWT-токена
			string accessToken = GenerateJwtToken(username, userInfo.Role);
			string refreshToken = GenerateRefreshToken();
			return (accessToken, refreshToken);
		}
		else
		{
			throw new UnauthorizedAccessException();
		}
	}

	/* Регистрация отключена в текущей версии */

	private static string GenerateRefreshToken ()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
	}
	private string GenerateJwtToken (string username, string role)
	{
		byte [] key = Encoding.UTF8.GetBytes(configuration ["Jwt:Key"]);

		JwtSecurityTokenHandler tokenHandler = new();

		SecurityTokenDescriptor tokenDescriptor = new()
		{
			Subject = new ClaimsIdentity(new [] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, role) }),
			Audience = "AUDIENCE",
			Issuer = "ISSUER",
			Expires = DateTime.UtcNow.AddMinutes(3),
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

	private static string ComputeSha512Hex (string input)
	{
		byte [] bytes = Encoding.UTF8.GetBytes(input);
		byte [] hash = SHA512.HashData(bytes);
		StringBuilder sb = new(hash.Length * 2);
		foreach (byte b in hash)
		{
			_ = sb.Append(b.ToString("x2"));
		}
		return sb.ToString();
	}

	private sealed class UserEntry
	{
		public string? Login { get; set; }
		public string? PasswordSha512 { get; set; }
		public string? Role { get; set; }
	}

    private static Dictionary<string, (string PasswordSha512, string Role)> BuildUsers (IConfiguration configuration)
    {
        IConfigurationSection usersSection = configuration.GetSection("users");
        if (!usersSection.Exists())
        {
            return new Dictionary<string, (string PasswordSha512, string Role)>(StringComparer.OrdinalIgnoreCase);
        }

        Dictionary<string, (string PasswordSha512, string Role)> result = new(StringComparer.OrdinalIgnoreCase);
        foreach (IConfigurationSection child in usersSection.GetChildren())
        {
            string? login = child["login"];
            string? passwordSha512 = child["passwordSha512"];
            string role = child["role"] ?? "User";
            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(passwordSha512))
            {
                result[login] = (passwordSha512, role);
            }
        }
        return result;
    }
}
