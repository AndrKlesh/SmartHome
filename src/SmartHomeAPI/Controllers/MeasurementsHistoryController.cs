#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// Контроллер истории измерений
/// </summary>
/// <param name="measuresStorageService"></param>
[ApiController]
[Route("api/[controller]")]
public sealed class MeasurementsHistoryController (MeasuresStorageService measuresStorageService) : Controller
{
	/// <summary>
	/// Получить историю измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <param name="startDate">Дата начала</param>
	/// <param name="endDate">Дата колнца</param>
	/// <returns></returns>
	[HttpGet]
	public async Task<ActionResult<List<MeasuresHistoryDTO>>> Get
	(
			[FromQuery] Guid measurementId,
			[FromQuery] DateTime startDate,
			[FromQuery] DateTime endDate
	)
	{
		try
		{
			startDate = startDate.ToUniversalTime();
			endDate = endDate.ToUniversalTime();

			IReadOnlyList<MeasuresHistoryDTO> measurements = await measuresStorageService
				.GetMeasurementHistory(measurementId, startDate, endDate)
				.ConfigureAwait(false);

			if (measurements == null || measurements.Count == 0)
			{
				return NotFound(new { message = "No measurements found for the given topic and date range." });
			}

			return Ok(measurements);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
		}
	}
}
