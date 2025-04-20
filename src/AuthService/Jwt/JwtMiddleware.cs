using AuthService.Services;

namespace AuthService.Jwt;

public sealed class JwtMiddleware (RequestDelegate next, LoginService loginService)
{
	public async Task Invoke (HttpContext context)
	{
		string path = context.Request.Path.Value;

		// Проверка url, следующие проверки пройдут, только в том случае,
		// если пользовательский путь начинается с /SmartHome/dashboard/
		// если нет, делаем await next
		if (!path?.StartsWith("/SmartHome/dashboard/", StringComparison.OrdinalIgnoreCase) ?? true)
		{
			await next(context).ConfigureAwait(false);
			return;
		}

		string token = context?.Request.Cookies ["jwt"];
		if (!loginService.CheckToken(token, out string usernameFromToken))
		{
			context.Response.StatusCode = 401;
			await context.Response.WriteAsync("Invalid token").ConfigureAwait(false);
			return;
		}

		context.Items ["User"] = usernameFromToken;

		context.Items ["User"] = "AuthenticatedUser";

		await next(context).ConfigureAwait(true);
	}
}
