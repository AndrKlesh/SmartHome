#pragma warning disable CA1515

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging.Console;
using Scalar.AspNetCore;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;

namespace SmartHomeAPI;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
			.AddDebug()
			.AddConfiguration(builder.Configuration.GetSection("Logging"));

		_ = builder.Services
			// Configuration
			.Configure<List<SubscriptionDomain>>(builder.Configuration.GetSection("Subscriptions"))
			.Configure<ConcurrentDictionary<string, Guid>>(builder.Configuration.GetSection("Links"))

			// Repositories
			.AddSingleton<MeasuresRepository>()
			.AddSingleton<SubscriptionRepository>()
			.AddSingleton<MeasuresLinksRepository>()
			.AddSingleton<SvgImagesRepository>()

			// Services
			.AddSingleton<MeasuresStorageService>()
			.AddSingleton<SubscriptionService>()
			.AddSingleton<MeasuresLinksService>()
			.AddSingleton<SvgImagesService>()
			.AddHostedService<MeasuresReceiverService>()

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
		app.Urls.Add(builder.Configuration ["Urls"]);

		app.Run();
	}
}
