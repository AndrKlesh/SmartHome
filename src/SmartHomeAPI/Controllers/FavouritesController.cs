using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavouritesController(MeasuresStorageService _measuresStorageService,
								  FavoritesMeasurementssService _favoritesMeasuresService) : Controller
{
	[HttpGet("latest")]
	public async Task<ActionResult<List<MeasureDTO>>> GetLatestFavouriteMeasurements ()
	{

		List<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync().ConfigureAwait(false);
		return Ok(latestMeasurements.Where(lm => lm.IsFavourite).ToList());
	}

	[HttpPost("toggleFavourite")]
	public async Task<IActionResult> ToggleFavourite ([FromBody] FavoritesDTO toggleFavouriteDto)
	{
		try
		{
			await _favoritesMeasuresService.AddFavoriteMeasureAsync(toggleFavouriteDto).ConfigureAwait(false);
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
