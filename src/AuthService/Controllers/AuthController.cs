#pragma warning disable CA1515
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}
