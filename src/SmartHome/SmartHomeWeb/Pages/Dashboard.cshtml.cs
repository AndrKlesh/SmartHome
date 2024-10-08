using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartHomeWeb.Services;

namespace SmartHomeWeb.Pages
{
  public class DashboardModel(MeasuresUIPreprocessingService _measuresUI) : PageModel
  {
    public void OnGet()
    {
      MeasuresUI = _measuresUI.EnumerateAllMeasures();
    }

    public IEnumerable<string>? MeasuresUI { get; private set; }
  }
}
