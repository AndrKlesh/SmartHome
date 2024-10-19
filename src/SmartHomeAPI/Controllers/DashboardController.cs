using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController (MeasuresStorageService measuresStorageService) : ControllerBase
{
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;

	[HttpGet("latest")]
	public async Task<ActionResult<List<MeasureWithFavouriteFlagDTO>>> GetLatestMeasurements ()
	{
		List<MeasureWithFavouriteFlagDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync();
		return Ok(latestMeasurements);
	}

	[HttpPost("toggleFavourite")]
	public async Task<IActionResult> ToggleFavourite ([FromBody] TopicDTO toggleFavouriteDto)
	{
		try
		{
			await _measuresStorageService.ToggleFavouriteAsync(toggleFavouriteDto.TopicName, toggleFavouriteDto.IsFavourite);
			return Ok(new { message = "Favourite state updated successfully." });
		}
		catch (ArgumentException ex)
		{
			return NotFound(new { message = ex.Message });
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
		}
	}
}
