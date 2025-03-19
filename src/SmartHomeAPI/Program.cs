#pragma warning disable CA1515

using Scalar.AspNetCore;
using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;

namespace SmartHomeAPI;

internal sealed class Program
{
	internal static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		_ = builder.Services
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

		if (app.Environment.IsDevelopment())
		{
			_ = app.MapOpenApi();
			_ = app.MapScalarApiReference();
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();
		app.Urls.Add("https://*:7098");
		app.Run();
	}
}
