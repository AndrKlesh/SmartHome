using Microsoft.Extensions.Logging.Console;
using Scalar.AspNetCore;

namespace MeasuresReceiverService.API;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		_ = builder.WebHost.UseKestrel();

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

		// TODO: Тут мы должны зарегестрировать абстракцию и имплементацию. ССылки на них уже добавлены в проект

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
