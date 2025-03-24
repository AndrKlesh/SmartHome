#pragma warning disable CA1515

using System.Reflection;
using Microsoft.Extensions.Logging.Console;
using Scalar.AspNetCore;
using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;

namespace SmartHomeAPI;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
			// TODO Не хардкодить сообщения логгера, а вынести их в локализацию
			.AddSingleton<MeasuresStorageService>()
			.AddSingleton<MeasuresRepository>()
			.AddSingleton<SubscriptionService>()
			.AddSingleton<SubscriptionRepository>()
			.AddSingleton<MeasuresLinksService>()
			.AddSingleton<MeasuresLinksRepository>()
			.AddSingleton<SvgImagesService>()
			.AddSingleton<SvgImagesRepository>()
			.AddHostedService<MeasuresReceiverService>()
			.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()))
			.AddControllers();

		_ = builder.Services.AddOpenApi();

		WebApplication app = builder.Build();

		ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

		if (app.Environment.IsDevelopment())
		{
			_ = app.MapOpenApi();
			_ = app.MapScalarApiReference();
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();
		app.Urls.Add("https://*:7098");

		string projectName = Assembly.GetExecutingAssembly().GetName().Name;
		logger.LogInformation("{ProjectName} запущен", projectName);
		app.Run();
	}
}
