using System.Collections.Immutable;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public class FavouritesMeasuresRepository
{
	public Task AddFavoriteMeasureAsync (FavoritesDomain favoritesMeasure)
	{
		var temp = _favoritesMeasuresIds;
		//TODO: убрать
		if (temp.Contains(favoritesMeasure.MeasureId))
		{
			_favoritesMeasuresIds = temp.Remove(favoritesMeasure.MeasureId);
		}
		else
		{
			_favoritesMeasuresIds = temp.Add(favoritesMeasure.MeasureId);
		}
		return Task.CompletedTask;
	}

	public Task RemoveFavoriteMeasureAsync (FavoritesDomain favoritesMeasure)
	{
		_favoritesMeasuresIds = _favoritesMeasuresIds.Remove(favoritesMeasure.MeasureId);
		return Task.CompletedTask;
	}

	public Task ClearFavoriteMeasureAsync ()
	{
		_favoritesMeasuresIds = _favoritesMeasuresIds.Clear();
		return Task.CompletedTask;
	}

	//TODO: Убрать ToArray()
	public Task<IReadOnlyList<FavoritesDomain>> GetFavoritesMeasuresIdsAsync ()
	{
		return Task.FromResult((IReadOnlyList<FavoritesDomain>) _favoritesMeasuresIds.Select(id => new FavoritesDomain() { MeasureId = id }).ToArray());
	}

	private ImmutableList<string> _favoritesMeasuresIds = ImmutableList<string>.Empty;
}
