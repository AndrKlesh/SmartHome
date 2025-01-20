#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// Контроллер ссылок измерений.
/// Ссылки представляют собой путь, как в файловой системе.
/// По ним измерения ведется группировка измерений в UI.
/// </summary>
/// <param name="measuresLinksRepository">Репозиторий связи id_измерения:ссылка</param>
[ApiController]
[Route("api/[controller]")]
public class MeasuresLinksController(MeasuresLinksService measuresLinksService) : Controller
{
	private readonly MeasuresLinksService _measuresLinksService = measuresLinksService;

	/// <summary>
	/// Получить следующий уровень пути.
	/// Если path пустой, то загружается корень.
	/// </summary>
	/// <param name="path">Предыдущий путь</param>
	/// <returns></returns>
	[HttpGet("nextLayer")]
	public async Task<ActionResult<string []>> GetNextMeasurementsLayer ([FromQuery]string? path)
	{
		string[] layer = await _measuresLinksService.LoadNextMeasurementsLayer(path).ConfigureAwait(false);
		return Ok(layer);
	}
}
