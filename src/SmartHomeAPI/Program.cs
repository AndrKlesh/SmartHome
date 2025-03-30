#pragma warning disable CA1515

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Scalar.AspNetCore;
using SmartHomeAPI.Data;
using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;
using SwaggerThemes;

namespace SmartHomeAPI;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		ConfigureLogging(builder);
		ConfigureDatabase(builder);
		ConfigureServices(builder);
		ConfigureOpenApi(builder);

		WebApplication app = builder.Build();

		if (args.Contains("--migrate"))
		{
			ApplyMigrations(app);
		}

		ConfigureApp(app);

		string projectName = Assembly.GetExecutingAssembly().GetName().Name;
		ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
		logger.LogInformation("{ProjectName} запущен", projectName);

		app.Run();
	}

	private static void ConfigureLogging (WebApplicationBuilder builder)
	{
		_ = builder.Logging.ClearProviders()
			.AddSimpleConsole(options =>
			{
				options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff zzz] ";
				options.UseUtcTimestamp = false;
				options.SingleLine = true;
				options.ColorBehavior = LoggerColorBehavior.Enabled;
			})
			.AddDebug()
			.AddConfiguration(builder.Configuration.GetSection("Logging"));
	}

	private static void ConfigureDatabase (WebApplicationBuilder builder)
	{
		string connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
		if (!string.IsNullOrEmpty(connectionString))
		{
			_ = builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
		}
	}

	private static void ConfigureServices (WebApplicationBuilder builder)
	{
		_ = builder.Services
			.AddSingleton<MeasuresStorageService>()
			.AddScoped<MeasuresRepository>()
			.AddSingleton<SubscriptionService>()
			.AddSingleton<SubscriptionRepository>()
			.AddSingleton<MeasuresLinksService>()
			.AddSingleton<MeasuresLinksRepository>()
			.AddSingleton<SvgImagesService>()
			.AddSingleton<SvgImagesRepository>()
			.AddHostedService<MeasuresReceiverService>()
			.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()))
			.AddControllers();
	}

	private static void ConfigureOpenApi (WebApplicationBuilder builder)
	{
		_ = builder.Services.AddOpenApiDocument(config =>
		{
			config.Title = "SmartHomeAPI";
			config.Version = "v1";
		});
	}

	private static void ApplyMigrations (WebApplication app)
	{
		using IServiceScope scope = app.Services.CreateScope();
		AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		dbContext.Database.Migrate();
	}

	private static void ConfigureApp (WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			_ = app.UseOpenApi();
			_ = app.UseSwaggerUi(settings => settings.CustomInlineStyles = SwaggerTheme.GetSwaggerThemeCss(Theme.UniversalDark));
			_ = app.MapScalarApiReference();
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();
		app.Urls.Add("https://*:7098");
	}
}
