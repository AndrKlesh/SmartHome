#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController (MeasuresStorageService measuresStorageService) : ControllerBase
{
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;

	[HttpGet("latest/{mask}")]
	public async Task<ActionResult<List<MeasureDTO>>> GetLatestMeasurements (string mask)
	{

		List<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync($"{mask}*").ConfigureAwait(false);
		return Ok(latestMeasurements);
	}
}

