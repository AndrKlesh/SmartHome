#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController (MeasuresStorageService _measuresStorageService,
								  SubscriptionService _subscriptionService,
								  FavoritesMeasurementssService _favoritesMeasuresService) : ControllerBase
{
	[HttpGet("latest")]
	public async Task<ActionResult<List<MeasureDTO>>> GetLatestMeasurements ()
	{

		List<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync().ConfigureAwait(false);
		IReadOnlyList<FavoritesDTO> favourites = await _favoritesMeasuresService.GetFavoritesMeasuresAsync().ConfigureAwait(false);
		foreach (MeasureDTO item in latestMeasurements)
		{
			SubscriptionDTO? subscription = await _subscriptionService.GetSubscriptionByMeasurementIdAsync(item.MeasurementId)
																	  .ConfigureAwait(false);
			if (subscription is null)
			{
				continue;
			}
			item.Name = subscription.MeasurementName;
			item.Units = subscription.Unit;
			item.IsFavourite = favourites.Any(f => f.MeasurementId == item.MeasurementId);
		}
		return Ok(latestMeasurements);
	}
}
