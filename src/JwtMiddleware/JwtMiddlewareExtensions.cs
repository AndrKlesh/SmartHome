﻿using Microsoft.AspNetCore.Builder;

namespace JwtMiddleware;

public static class JwtMiddlewareExtensions
{
	public static IApplicationBuilder UseJwtMiddleware (this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<JwtMiddleware>();
	}
}