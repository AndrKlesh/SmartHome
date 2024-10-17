using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartHomeWeb.Pages;

public class SettingsModel (ILogger<SettingsModel> logger) : PageModel
{
	private readonly ILogger<SettingsModel> _logger = logger;

	public void OnGet ()
	{
	}
}
