#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// Контроллер Dashboard'ов
/// </summary>
/// <param name="measuresStorageService"></param>
[ApiController]
[Route("api/[controller]")]
public class DashboardController (MeasuresStorageService measuresStorageService) : ControllerBase
{
	/// <summary>
	/// Получить последние измерения
	/// </summary>
	/// <param name="mask">Маска ссылок на измерения
	/// Например, получить измерения из группы Общие: mask = "Общие/*"</param>
	/// <returns></returns>
	[HttpGet("latest/{mask}")]
	public async Task<ActionResult<IReadOnlyList<MeasureDTO>>> GetLatestMeasurements (string mask)
	{

		IReadOnlyList<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync($"{mask}*").ConfigureAwait(false);
		return Ok(latestMeasurements);
	}
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;
}

