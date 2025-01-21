#pragma warning disable CA1515
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
	public async Task<string []> LoadNextMeasurementsLayer (string? path)
	{
		path ??= string.Empty;
		string mask = string.IsNullOrWhiteSpace(path) ? AllMask : path + MoreMask;
		IReadOnlyList<KeyValuePair<string, Guid>> links = await _measuresLinksRepository.FindLinksByMaskAsync(mask).ConfigureAwait(false);
		string [] sublayer = links.Select(l =>
		{
			int index = l.Key.IndexOf(path, StringComparison.Ordinal);
			string subpath = index < 0 ? l.Key : l.Key.Remove(index, path.Length);
			subpath = subpath.TrimStart('/');
			index = subpath.IndexOf('/', StringComparison.InvariantCultureIgnoreCase);
			return index < 0 ? subpath : subpath.Substring(0, index);
		}).Distinct()
		  .ToArray();
		return sublayer;
	}

	private readonly MeasuresLinksRepository _measuresLinksRepository = measuresLinksRepository;
	private const string AllMask = ".*";
	private const string MoreMask = "*";
}
