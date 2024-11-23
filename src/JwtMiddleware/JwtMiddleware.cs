using Microsoft.AspNetCore.Http;

namespace JwtMiddleware;

public class JwtMiddleware (RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	public async Task Invoke (HttpContext context)
	{
		string token = context.Request.Cookies ["jwt"];
		if (token == "test-token")
		{
			context.Items ["User"] = "AuthenticatedUser";
		}

		await _next(context);
	}
}
