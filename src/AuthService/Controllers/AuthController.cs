#pragma warning disable CA1515

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController (IConfiguration configuration) : ControllerBase
{
	private readonly IConfiguration _configuration = configuration;

	[HttpPost("login")]
	public IActionResult Login ([FromBody] User user)
	{
		if (user is null || user.Username is null || user.Password is null)
		{
			return BadRequest();
		}
		else if (user.Username == "user" && user.Password == "user")
		{
			// Генерация JWT-токена
			string token = GenerateJwtToken(user.Username);

			Response.Cookies.Append("jwt", token);
			return Ok(token);
		}
		else
		{
			return Unauthorized();
		}
	}

	private string GenerateJwtToken (string username)
	{
		byte [] key = Encoding.UTF8.GetBytes(_configuration ["Jwt:Key"]);

		JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

		SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
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
