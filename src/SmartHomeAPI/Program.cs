#pragma warning disable CA1515

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
			.AddSingleton<MeasuresLinksRepository>()
			.AddSingleton<SubscriptionService>()
			.AddSingleton<SubscriptionRepository>()
			.AddSingleton<MeasurementRepository>()
			.AddSingleton<FavouritesMeasuresRepository>()
			.AddSingleton<FavoritesMeasurementssService>()
			.AddHostedService<MeasuresReceiverService>()
			.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()))
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddControllers();

		WebApplication app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			_ = app.UseSwagger();
			_ = app.UseSwaggerUI();
		}

		_ = app.UseHttpsRedirection();
		_ = app.UseCors("AllowAll");
		_ = app.MapControllers();
		app.Urls.Add("https://*:7098");
		app.Run();
	}
}
