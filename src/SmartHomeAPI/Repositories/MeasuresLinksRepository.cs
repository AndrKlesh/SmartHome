#pragma warning disable CA1515

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий ссылок на типы измерений
/// </summary>
public sealed class MeasuresLinksRepository
{
	private readonly ILogger<MeasuresLinksRepository> logger;
	private readonly IOptionsMonitor<Dictionary<string, Guid>> optionsMonitor;
	private ImmutableDictionary<string, Guid> _links;

	public MeasuresLinksRepository (ILogger<MeasuresLinksRepository> logger, IOptionsMonitor<Dictionary<string, Guid>> optionsMonitor)
	{
		this.logger = logger;
		this.optionsMonitor = optionsMonitor;

		_links = optionsMonitor?.CurrentValue.ToImmutableDictionary() ?? ImmutableDictionary<string, Guid>.Empty;

		_ = optionsMonitor.OnChange(updatedLinks => ImmutableInterlocked.Update(ref _links, _ => updatedLinks?.ToImmutableDictionary() ?? ImmutableDictionary<string, Guid>.Empty));
	}

	/// <summary>
	/// Получить ID типа измерения по ссылке на него.
	/// </summary>
	/// <param name="path">Путь ссылки</param>
	/// <returns></returns>
	public Task<Guid> GetMeasurementIdAsync (string path)
	{
		logger.LogInformation("Получение ID измерения по пути '{Path}'...", path);

		if (_links.TryGetValue(path, out Guid measurementId))
		{
			logger.LogInformation("Получен ID измерения '{MeasurementId}' по пути '{Path}'", measurementId, path);
			return Task.FromResult(measurementId);
		}
		else
		{
			logger.LogWarning("Не найден ID измерения по пути '{Path}'", path);
			return Task.FromResult(Guid.Empty);
		}
	}

	/// <summary>
	/// Поиск связи (Путь <-> Guid) по маске.
	/// Например,
	/// 1) Общие/Температура воздуха/*
	/// 2) */Температура*
	/// </summary>
	/// <param name="mask">Маска/регулярное выражение ссылок</param>
	/// <returns></returns>
	public Task<IReadOnlyList<KeyValuePair<string, Guid>>> FindLinksByMaskAsync (string mask)
	{
		logger.LogInformation("Получение ссылок по маске '{Mask}'...", mask);
		KeyValuePair<string, Guid> [] results = _links.Where(item => Regex.IsMatch(item.Key, mask)).ToArray();

		if (results.Length == 0)
		{
			logger.LogWarning("Не найдено соответствий по маске '{Mask}'", mask);
		}
		else
		{
			logger.LogInformation("Найдено {Count} соответствий по маске '{Mask}'", results.Length, mask);
		}

		return Task.FromResult((IReadOnlyList<KeyValuePair<string, Guid>>) results);
	}

	/// <summary>
	/// Добавить ссылку
	/// (Путь <-> Guid). Например, (Общие/Температура воздуха <-> 24FE134B-4CBF-4EB9-A811-2720D4315146)
	/// </summary>
	/// <param name="path">Путь ссылки. Представляет собой путь, как в файловой системе</param>
	/// <param name="measurementId">ID типа измерения</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task AddMeasurementLinkAsync (string path, Guid measurementId)
	{
		throw new NotImplementedException("Добавление ссылки не реализовано");

		/*
		logger.LogInformation("Добавление ссылки: '{Path}' -> '{MeasurementId}'...", path, measurementId);
		if (!_links.TryAdd(path, measurementId))
		{
			logger.LogWarning("Ссылка по пути '{Path}' не добавлена", path);
		}

		return Task.CompletedTask;
		*/
	}

	/// <summary>
	/// Удалить ссылку на тип измерения по пути
	/// </summary>
	/// <param name="path">Путь/Ссылка</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task DeleteMeasurementLinkAsync (string path)
	{
		throw new NotImplementedException("Удаление ссылки по пути не реализовано");

		/*
		logger.LogInformation("Удаление ссылки по пути '{Path}'...", path);
		if (_links.TryRemove(path, out _))
		{
			logger.LogInformation("Ссылка по пути '{Path}' удалена", path);
		}
		else
		{
			logger.LogWarning("Не найдена ссылка для удаления по пути '{Path}'", path);
		}

		return Task.CompletedTask;
		*/
	}

	/// <summary>
	/// Удалить ссылки на измерения по ID типа измерения
	/// Внимание, удаляются все ссылки на ID типа измерения
	/// </summary>
	/// <param name="measurementId">ID типа измерения</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task DeleteMeasurementLinkAsync (Guid measurementId)
	{
		throw new NotImplementedException("Удаление всех ссылок для measurementID не реализовано");

		/*
		logger.LogInformation("Удаление всех ссылок для measurementID = '{MeasurementId}'...", measurementId);

		Dictionary<string, Guid> linksToRemove = _links.Where(x => x.Value == measurementId).ToDictionary();
		foreach (KeyValuePair<string, Guid> link in linksToRemove)
		{
			if (_links.TryRemove(link.Key, out _))
			{
				logger.LogInformation("Ссылка по пути '{Path}' удалена", link.Key);
			}
			else
			{
				logger.LogWarning("Ссылка по пути '{Path}' не удалена", link.Key);
			}
		}

		return Task.CompletedTask;
		*/
	}
}
