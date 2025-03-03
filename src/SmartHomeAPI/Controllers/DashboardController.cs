#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// Коонтроллер Dashboard'ов
/// </summary>
/// <param name="measuresStorageService"></param>
[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController (MeasuresStorageService measuresStorageService) : Controller
{
	/// <summary>
	/// Получить последние измерения
	/// </summary>
	/// <param name="mask">Маска ссылок на типы измерений
	/// Например, получить измерения из группы "Общие": mask = "Общие/*"</param>
	/// <returns></returns>
	[HttpGet("latest/{mask}")]
	public async Task<ActionResult<IReadOnlyList<MeasureDTO>>> GetLatestMeasurements (string mask)
	{
		IReadOnlyList<MeasureDTO> latestMeasurements = await measuresStorageService.GetLatestMeasurementsAsync(mask).ConfigureAwait(false);
		return Ok(latestMeasurements);
	}

	/// <summary>
	/// Подписаться на последние значения измерений
	/// </summary>
	/// <param name="mask"></param>
	/// <returns></returns>
	[HttpGet("latestPoll/{mask}")]
	public async Task<ActionResult<IReadOnlyList<MeasureDTO>>> SubscribeToLatestMeasurements (string mask)
	{
		IReadOnlyList<MeasureDTO> latestMeasurements = await measuresStorageService.SubscribeToLatestMeasurementsAsync(mask).ConfigureAwait(false);
		return Ok(latestMeasurements);
	}
}
