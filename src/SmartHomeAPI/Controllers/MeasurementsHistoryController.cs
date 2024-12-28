using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsHistoryController (MeasuresStorageService _measuresStorageService) : Controller
{
	[HttpGet]
	public async Task<ActionResult<List<MeasuresHistoryDTO>>> Get (
			[FromQuery] Guid measurementId,
			[FromQuery] DateTime startDate,
			[FromQuery] DateTime endDate)
	{
		try
		{
			startDate = startDate.ToUniversalTime();
			endDate = endDate.ToUniversalTime();
			List<MeasuresHistoryDTO> measurements = await _measuresStorageService.GetMeasurementsByTopicAndDateRangeAsync(measurementId, startDate, endDate).ConfigureAwait(false);

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
