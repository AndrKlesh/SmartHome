using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController (TokenService tokenService) : ControllerBase
{
	private readonly TokenService _tokenService = tokenService;

	[HttpPost("login")]
	public async Task<IActionResult> Login ([FromBody] User user)
	{
		string? token = await _tokenService.AuthenticateAsync(user.Username, user.Password);
		if (token == null)
		{
			return Unauthorized("Invalid username or password");
		}

		return Ok(new { Token = token });
	}
}
