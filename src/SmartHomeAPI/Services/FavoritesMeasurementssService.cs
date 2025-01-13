using System.Diagnostics.Metrics;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

public class FavoritesMeasurementssService (FavouritesMeasuresRepository _favoritesMeasuresRepository)
{
	public Task AddFavoriteMeasureAsync (FavoritesDTO favoritesMeasure)
	{
		return _favoritesMeasuresRepository.AddFavoriteMeasureAsync(new Entities.FavoritesDomain() { MeasureId = favoritesMeasure.MeasurementId });
	}

	public Task RemoveFavoriteMeasure (FavoritesDTO favoritesMeasure)
	{
		return _favoritesMeasuresRepository.RemoveFavoriteMeasureAsync(new Entities.FavoritesDomain() { MeasureId = favoritesMeasure.MeasurementId });
	}

	public Task ClearFavoriteMeasure ()
	{
		return _favoritesMeasuresRepository.ClearFavoriteMeasureAsync();
	}

	//TODO: Убрать ToArray()
	public async Task<IReadOnlyList<FavoritesDTO>> GetFavoritesMeasuresAsync ()
	{
		var fMeasures = await _favoritesMeasuresRepository.GetFavoritesMeasuresIdsAsync().ConfigureAwait(false);
		return fMeasures.Select(fMeasure => new FavoritesDTO() { MeasurementId = fMeasure.MeasureId }).ToArray();
	}
}
