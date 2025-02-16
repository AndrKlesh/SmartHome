#pragma warning disable CA1515

using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;
using SwaggerThemes;

namespace SmartHomeAPI;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		_ = builder.Services
			.AddSingleton<MeasuresStorageService>()
			.AddSingleton<MeasuresLinksRepository>()
			.AddSingleton<SubscriptionService>()
			.AddSingleton<SubscriptionRepository>()
			.AddSingleton<MeasurementRepository>()
			.AddSingleton<MeasuresLinksService>()
			.AddHostedService<MeasuresReceiverService>()
			.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()))
			.AddControllers();

		_ = builder.Services.AddOpenApiDocument(config =>
		{
			config.Title = "SmartHomeAPI";
			config.Version = "v1";
		});

		WebApplication app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			_ = app.UseOpenApi();
			_ = app.UseSwaggerUi(settings => settings.CustomInlineStyles = SwaggerTheme.GetSwaggerThemeCss(Theme.UniversalDark));
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();
		app.Urls.Add("https://*:7098");
		app.Run();
	}
}
