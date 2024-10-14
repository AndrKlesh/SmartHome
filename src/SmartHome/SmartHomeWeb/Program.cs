using SmartHomeWeb.Services;

internal sealed class Program
{
	private static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();

		builder.Services.AddSingleton<MeasuresUIPreprocessingService>();
		builder.Services.AddSingleton<IMeasuresStorageService, MeasuresStorageService>();
		builder.Services.AddHostedService<MeasuresReceiverService>();

		WebApplication app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
		}

		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.MapRazorPages();

		app.Run();
	}
}
