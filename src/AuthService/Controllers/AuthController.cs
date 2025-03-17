#pragma warning disable CA1515
using System.Security.Claims;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowFrontend")]
public sealed class AuthController (LoginService loginService) : ControllerBase
{
	//private readonly LoginService _loginService = loginService;

	[HttpPost("login")] // Авторизация и генерация JWT
	public IActionResult Login ([FromBody] User user)
	{
		if (user is null || user.Username is null || user.Password is null)
		{
			return BadRequest();
		}

		try
		{
			string token = loginService.Login(user.Username, user.Password);
			Response.Cookies.Append("jwt", token, new CookieOptions
			{
				SameSite = SameSiteMode.None
			});
			return Ok();
		}
		catch (Exception)
		{
			return Unauthorized();
		}
	}

	[Authorize] // Аутентификации через JWT
	[HttpGet("me")]
	public IActionResult GetCurrentUser ()
	{
		string username = User.FindFirst(ClaimTypes.Name)?.Value;

		if (username is null)
		{
			return Unauthorized(new { message = "Токен недействителен" });
		}

		return Ok(new { username });
	}
}
