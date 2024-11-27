#pragma warning disable CA1515

using SmartHomeAPI.Repositories;
using SmartHomeAPI.Services;

namespace SmartHomeAPI;

public class Program
{
	public static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		_ = builder.Services.AddSingleton<MeasuresStorageService>();
		_ = builder.Services.AddSingleton<SubscriptionService>();
		_ = builder.Services.AddSingleton<SubscriptionRepository>();
		_ = builder.Services.AddSingleton<MeasurementRepository>();
		_ = builder.Services.AddSingleton<TopicRepository>();
		_ = builder.Services.AddHostedService<MeasuresReceiverService>();

		_ = builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowAll",
				policy =>
				{
					_ = policy.AllowAnyOrigin()  // Разрешаем запросы с любых источников
						  .AllowAnyMethod()
						  .AllowAnyHeader();
				});
		});

		_ = builder.Services.AddControllers();
		_ = builder.Services.AddEndpointsApiExplorer();
		_ = builder.Services.AddSwaggerGen();

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
