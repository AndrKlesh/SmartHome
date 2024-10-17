using SmartHomeWeb.Services;

internal sealed class Program
{
	private static void Main ()
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder();

		builder.Services.AddRazorPages();

		builder.Services.AddSingleton<MeasuresUIPreprocessingService>();
		builder.Services.AddSingleton<IMeasuresStorageService, MeasuresStorageService>();
		builder.Services.AddHostedService<MeasuresReceiverService>();

		WebApplication app = builder.Build();

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
