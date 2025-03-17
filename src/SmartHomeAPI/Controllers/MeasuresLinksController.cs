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
public sealed class MeasuresLinksController (MeasuresLinksService measuresLinksService, ILogger<MeasuresLinksController> logger) : Controller
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
		logger.LogInformation("Запрос следующего уровня измерений для пути: {Path}", path ?? "корень");

		try
		{
			IReadOnlyList<LinkDTO> layer = await measuresLinksService.LoadNextMeasurementsLayer(path).ConfigureAwait(false);

			if (layer.Count == 0)
			{
				logger.LogWarning("Для пути {Path} не найдено данных", path ?? "корень");
			}

			return Ok(layer);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при получении следующего уровня измерений для пути {Path}", path ?? "корень");
			return StatusCode(500, new { message = "Внутренняя ошибка сервера", error = ex.Message });
		}
	}
}
