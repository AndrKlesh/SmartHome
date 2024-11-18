
namespace AuthService;

public class Program
{
	public static void Main (string [] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		_ = builder.Services.AddControllers();
		_ = builder.Services.AddEndpointsApiExplorer();

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

		WebApplication app = builder.Build();

		_ = app.UseAuthorization();

		_ = app.MapControllers();

		app.Run();
	}
}
