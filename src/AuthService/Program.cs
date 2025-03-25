using System.Text;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
		/*_ = builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowFrontend",
				policy =>
				{
					_ = policy.WithOrigins("http://localhost:5173")  // !!!Разрешаем запросы с фронта!!!
						  .AllowAnyMethod()
						  .AllowAnyHeader()
						  .AllowCredentials();
				});
		});*/

		IConfigurationSection jwtSettings = builder.Configuration.GetSection("Jwt");
		byte [] key = Encoding.UTF8.GetBytes(jwtSettings ["Key"]);

		/*_ = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings ["Issuer"],
			ValidAudience = jwtSettings ["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(key)
		};
	});*/

		WebApplication app = builder.Build();

		//_ = app.UseCors("AllowFrontend");
		_ = app.UseCors("AllowAll");

		//_ = app.UseAuthentication();

		//_ = app.UseAuthorization();

		_ = app.MapControllers();

		app.Run();
	}
}
