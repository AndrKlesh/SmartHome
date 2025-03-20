#pragma warning disable CA1515

using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис ссылок на типы измерений
/// </summary>
/// <param name="measuresLinksRepository"></param>
public sealed class MeasuresLinksService (MeasuresLinksRepository measuresLinksRepository, ILogger<MeasuresLinksService> logger)
{
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
		logger.LogInformation("Загрузка следующего уровня по пути: '{Path}'...", path);

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

		try
		{
			logger.LogInformation("Используем маску: {Mask} для поиска ссылок...", mask);
			IReadOnlyList<KeyValuePair<string, Guid>> links = await measuresLinksRepository.FindLinksByMaskAsync(mask).ConfigureAwait(false);

			LinkDTO [] sublayer = links
				.Select(link => CreateLinkDTO(link.Key, path))
				.DistinctBy(dto => dto.Path)
				.ToArray();

			logger.LogInformation("Найдено {Count} ссылок", sublayer.Length);

			return sublayer;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при загрузке следующего уровня ссылок");
			throw;
		}
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

		LinkDTO linkDTO = new();
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
