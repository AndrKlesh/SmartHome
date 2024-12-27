using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavouritesController(MeasuresStorageService _measuresStorageService,
								  SubscriptionService _subscriptionService,
								  FavoritesMeasurementssService _favoritesMeasuresService) : Controller
{
	[HttpGet("latest")]
	public async Task<ActionResult<List<MeasureDTO>>> GetLatestFavouriteMeasurements ()
	{

		List<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync().ConfigureAwait(false);
		IReadOnlyList<FavoritesDTO> favorites = await _favoritesMeasuresService.GetFavoritesMeasuresAsync().ConfigureAwait(false);

		List<MeasureDTO> res = new();
		foreach (MeasureDTO item in latestMeasurements)
		{
			if (favorites.Any(f => f.MeasurementId == item.MeasurementId))
			{
				SubscriptionDTO? subscription = await _subscriptionService.GetSubscriptionByMeasurementIdAsync(item.MeasurementId)
																		  .ConfigureAwait(false);
				if (subscription is null)
				{
					continue;
				}

				item.Name = subscription.MeasurementName;
				item.Units = subscription.Unit;
				item.IsFavourite = true;
				res.Add(item);
			}
		}

		return Ok(res);
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
