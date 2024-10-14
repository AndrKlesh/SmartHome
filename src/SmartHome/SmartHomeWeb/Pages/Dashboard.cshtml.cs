using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartHomeWeb.Services;

namespace SmartHomeWeb.Pages;

public class DashboardModel (MeasuresUIPreprocessingService measuresUI) : PageModel
{
	public void OnGet ()
	{
		MeasuresUI = measuresUI.EnumerateAllMeasures();
	}

	public IEnumerable<string>? MeasuresUI { get; private set; }
}
