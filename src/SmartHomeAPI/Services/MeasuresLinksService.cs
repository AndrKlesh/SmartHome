#pragma warning disable CA1515

using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис ссылок на типы измерений
/// </summary>
/// <param name="measuresLinksRepository"></param>
public class MeasuresLinksService (MeasuresLinksRepository measuresLinksRepository)
{
	private readonly MeasuresLinksRepository _measuresLinksRepository = measuresLinksRepository;
	private const string AllMask = ".*";
	private const string MoreMask = "*";

	/// <summary>
	/// Загрузить следующий уровень пути ссылки
	/// </summary>
	/// <param name="path">Предыдущий путь
	/// Если путь пустой - вернутся корневые ссылки</param>
	/// <returns></returns>
	public async Task<IReadOnlyList<LinkDTO>> LoadNextMeasurementsLayer (string? path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			path = string.Empty;
		}

		string mask;
		if (string.IsNullOrWhiteSpace(path))
		{
			mask = AllMask;
		}
		else
		{
			mask = $"{path}{MoreMask}";
		}

		IReadOnlyList<KeyValuePair<string, Guid>> links = await _measuresLinksRepository.FindLinksByMaskAsync(mask).ConfigureAwait(false);
		LinkDTO [] sublayer = links
			.Select(link => CreateLinkDTO(link.Key, path))
			.DistinctBy(dto => dto.Path)
			.ToArray();

		return sublayer;
	}

	private static LinkDTO CreateLinkDTO (string key, string path)
	{
		// Если путь содержится в ключе, удаляем его
		string subpath = key;
		int index = subpath.IndexOf(path, StringComparison.Ordinal);
		if (index >= 0)
		{
			subpath = subpath.Remove(index, path.Length);
		}

		subpath = subpath.TrimStart('/');
		index = subpath.IndexOf('/');

		LinkDTO linkDTO = new LinkDTO();
		if (index > 0)
		{
			linkDTO.Path = subpath.Substring(0, index);
			linkDTO.Mode = "d";
		}
		else
		{
			linkDTO.Path = subpath;
			linkDTO.Mode = string.Empty;
		}

		return linkDTO;
	}
}
