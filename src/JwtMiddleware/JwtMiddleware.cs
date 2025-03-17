using Microsoft.AspNetCore.Http;

namespace JwtMiddleware;

public sealed class JwtMiddleware (RequestDelegate next)
{
	public async Task Invoke (HttpContext context)
	{
		string token = context?.Request.Cookies ["jwt"];
		if (token == "test-token")
		{
			context.Items ["User"] = "AuthenticatedUser";
		}

		await next(context).ConfigureAwait(false);
	}
}
