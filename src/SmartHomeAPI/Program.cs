using SmartHomeAPI.Services;

namespace SmartHomeAPI;

public class Program
{
	public static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		_ = builder.Services.AddSingleton<MeasuresStorageService>();
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
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		_ = builder.Services.AddEndpointsApiExplorer();
		_ = builder.Services.AddSwaggerGen();

		WebApplication app = builder.Build();

		// Configure the HTTP request pipeline.
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
