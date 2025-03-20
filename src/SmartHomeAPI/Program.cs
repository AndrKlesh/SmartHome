#pragma warning disable CA1515

using System.Reflection;
using Microsoft.Extensions.Logging.Console;
using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;
using SwaggerThemes;

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

		string projectName = Assembly.GetExecutingAssembly().GetName().Name;

		_ = builder.Services.AddOpenApiDocument(config =>
		{
			config.Title = projectName;
			config.Version = "v1";
		});

		WebApplication app = builder.Build();

		ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

		if (app.Environment.IsDevelopment())
		{
			_ = app.UseOpenApi();
			_ = app.UseSwaggerUi(settings => settings.CustomInlineStyles = SwaggerTheme.GetSwaggerThemeCss(Theme.UniversalDark));
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();
		app.Urls.Add("https://*:7098");

		logger.LogInformation("{ProjectName} запущен", projectName);
		app.Run();
	}
}
