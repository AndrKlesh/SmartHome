using MQTTnet.AspNetCore;
using SmartHomeWeb.Services;

internal sealed class Program
{
	private static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();

		builder.WebHost.ConfigureKestrel(op =>
		{
			op.ListenAnyIP(1883, l => l.UseMqtt());
			op.ListenAnyIP(5000);
		});

		builder.Services.AddSingleton<MeasuresUIPreprocessingService>();
		builder.Services.AddSingleton<MeasuresStorageService>();
		builder.Services.AddHostedService<MeasuresReceiverService>();

		builder.Services.AddHostedMqttServer(optionsBuilder => optionsBuilder.WithDefaultEndpoint());

		builder.Services.AddMqttConnectionHandler();
		builder.Services.AddConnections();

		WebApplication app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
		}

		app.UseMqttServer(server =>
		{
		});

		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.MapRazorPages();

		app.Run();
	}
}
