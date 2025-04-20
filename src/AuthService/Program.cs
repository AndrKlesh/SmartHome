using System.Text;
using AuthService.Services;

namespace AuthService;

internal sealed class Program
{
	public static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		_ = builder.Services.AddSingleton<LoginService>();
		_ = builder.Services.AddControllers();
		_ = builder.Services.AddEndpointsApiExplorer();
		_ = builder.Services.AddAuthorization();
		_ = builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

		IConfigurationSection jwtSettings = builder.Configuration.GetSection("Jwt");
		byte [] key = Encoding.UTF8.GetBytes(jwtSettings ["Key"]);

		WebApplication app = builder.Build();

		_ = app.UseCors("AllowAll");

		_ = app.UseJwtMiddleware();

		_ = app.MapControllers();

		app.Run();
	}
}
