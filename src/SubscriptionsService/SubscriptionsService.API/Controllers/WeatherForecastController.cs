using Microsoft.AspNetCore.Mvc;

namespace SubscriptionsService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController (ILogger<WeatherForecastController> logger) : ControllerBase
{
	[HttpGet(Name = "test")]
	public bool Get ()
	{
		logger.LogInformation("Test");
		return true;
	}
}
