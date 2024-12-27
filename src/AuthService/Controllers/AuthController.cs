#pragma warning disable CA1515

using AuthService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
	[HttpPost("login")]
	public IActionResult Login ([FromBody] User user)
	{
		if (user is null || user.Username is null || user.Password is null)
		{
			return BadRequest();
		}
		else if (user.Username == "user" && user.Password == "user")
		{
			string token = "test-token";

			Response.Cookies.Append("jwt", token);
			return Ok(token);
		}
		else
		{
			return Unauthorized();
		}
	}
}
