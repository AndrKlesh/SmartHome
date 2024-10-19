using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartHomeWeb.Pages;

public class MeasureDetailsModel : PageModel
{
	public string? MeasureId { get; private set; }
	public void OnGet (string id)
	{
		MeasureId = id;
	}
}
