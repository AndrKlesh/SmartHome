#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController (MeasuresStorageService _measuresStorageService) : ControllerBase
{
	[HttpGet("latest")]
	public async Task<ActionResult<List<MeasureDTO>>> GetLatestMeasurements ()
	{

		List<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync().ConfigureAwait(false);
		return Ok(latestMeasurements);
	}
}
