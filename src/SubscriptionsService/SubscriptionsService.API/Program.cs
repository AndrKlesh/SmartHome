using Microsoft.Extensions.Logging.Console;
using Scalar.AspNetCore;
using SubscriptionsService.Abstractions.Repositories;
using SubscriptionsService.Abstractions.Services;
using SubscriptionsService.Implementation.Repositories;
using SubscriptionsService.Implementation.Services;

namespace SubscriptionsService.API;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		_ = builder.WebHost.UseKestrel();

		_ = builder.Services.Configure<List<SubscriptionsService.Abstractions.Entities.SubscriptionDomain>>(builder.Configuration.GetSection("Subscriptions"));

		_ = builder.Services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
		_ = builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();

		// Logging
		_ = builder.Logging
			.ClearProviders()
			.AddSimpleConsole(options =>
			{
				options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff zzz] ";
				options.UseUtcTimestamp = false;
				options.SingleLine = true;
				options.ColorBehavior = LoggerColorBehavior.Enabled;
			})
			.AddDebug();

		_ = builder.Services

			// CORS и API
			.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()))
			.AddOpenApi()
			.AddControllers();

		WebApplication app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			_ = app.MapOpenApi();
			_ = app.MapScalarApiReference();
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();

		app.Run();
	}
}
