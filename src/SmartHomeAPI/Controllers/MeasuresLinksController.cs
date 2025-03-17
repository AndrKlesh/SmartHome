#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// Контроллер ссылок измерений.
/// Ссылки представляют собой путь, как в файловой системе.
/// По ним измерения ведется группировка измерений в UI.
/// </summary>
/// <param name="measuresLinksService">Репозиторий связи id_измерения:ссылка</param>
[ApiController]
[Route("api/[controller]")]
public sealed class MeasuresLinksController (MeasuresLinksService measuresLinksService) : Controller
{
	/// <summary>
	/// Получить следующий уровень пути.
	/// Если path пустой, то загружается корень.
	/// </summary>
	/// <param name="path">Предыдущий путь</param>
	/// <returns></returns>
	[HttpGet("nextLayer")]
	public async Task<ActionResult<string []>> GetNextMeasurementsLayer ([FromQuery] string? path)
	{
		IReadOnlyList<LinkDTO> layer = await measuresLinksService.LoadNextMeasurementsLayer(path).ConfigureAwait(false);
		return Ok(layer);
	}
}
