#pragma warning disable CA1515
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController (LoginService loginService) : ControllerBase
{
	[HttpPost("login")] // Авторизация и генерация JWT
	public IActionResult Login ([FromBody] User user)
	{
		if (user is null || user.Username is null || user.Password is null)
		{
			return BadRequest();
		}

		try
		{
			(string accessToken, string refreshToken) = loginService.Login(user.Username, user.Password);

			Response.Cookies.Append("jwt", accessToken, new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Lax,
				Secure = Request.IsHttps,
			});

			Response.Cookies.Append("refresh", refreshToken, new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Lax,
				Secure = Request.IsHttps,
			});
			return Ok();
		}
		catch (Exception)
		{
			return Unauthorized();
		}
	}

	/*[HttpPost("register")]
	public IActionResult Register ([FromBody] User user)
	{
		if (user is null || user.Username is null || user.Password is null)
		{
			return BadRequest("Username and password are required");
		}

		bool success = loginService.Register(user.Username, user.Password);
		if (!success)
		{
			return Conflict("Username already exists");
		}

		return Ok("Registration successful");
	}*/
}
