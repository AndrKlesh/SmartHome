using AuthService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	[HttpPost("login")]
	public IActionResult Login ([FromBody] User user)
	{
		if (user.Username == "user" && user.Password == "user")
		{
			string token = "test-token";

			Response.Cookies.Append("jwt", token);
			return Ok(token);
		}

		return Unauthorized();
	}
}
