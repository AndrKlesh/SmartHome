#pragma warning disable CA1515
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис ссылок на типы измерений
/// </summary>
/// <param name="measuresLinksRepository"></param>
public class MeasuresLinksService(MeasuresLinksRepository measuresLinksRepository)
{
	/// <summary>
	/// Загрузить следующий уровень пути ссылки
	/// </summary>
	/// <param name="path">Предыдущий путь.
	/// Если путь пустой - вернутся корневые ссылки</param>
	/// <returns></returns>
	public async Task<IReadOnlyList<LinkDTO>> LoadNextMeasurementsLayer (string? path)
	{
		path ??= string.Empty;
		string mask = string.IsNullOrWhiteSpace(path) ? AllMask : path + MoreMask;
		IReadOnlyList<KeyValuePair<string, Guid>> links = await _measuresLinksRepository.FindLinksByMaskAsync(mask).ConfigureAwait(false);
		LinkDTO [] sublayer = links.Select(l =>
		{
			int index = l.Key.IndexOf(path, StringComparison.Ordinal);
			string subpath = index < 0 ? l.Key : l.Key.Remove(index, path.Length);
			subpath = subpath.TrimStart('/');
			index = subpath.IndexOf('/', StringComparison.InvariantCultureIgnoreCase);
			if (index > 0)
			{
				return new LinkDTO()
				{
					Path = subpath.Substring(0, index),
					Mode = "d"
				};
			}
			else
			{
				return new LinkDTO()
				{
					Path = subpath,
					Mode = string.Empty
				};
			}
		}).DistinctBy(l => l.Path)
		  .ToArray();
		return sublayer;
	}

	private readonly MeasuresLinksRepository _measuresLinksRepository = measuresLinksRepository;
	private const string AllMask = ".*";
	private const string MoreMask = "*";
}
