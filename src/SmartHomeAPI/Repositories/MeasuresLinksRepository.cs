#pragma warning disable CA1515

using System.Text.RegularExpressions;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий ссылок на типы измерений
/// </summary>
public class MeasuresLinksRepository
{
	/// <summary>
	/// Добавить ссылку
	/// (Путь <-> Guid). Например, (Общие/Температура воздуха <-> 24FE134B-4CBF-4EB9-A811-2720D4315146)
	/// </summary>
	/// <param name="path">Путь ссылки. Представляет собой путь, как в файловой системе</param>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task AddMeasureLinkAsync (string path, Guid measurementId)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Получить ид. типа измерения по ссылке на него.
	/// </summary>
	/// <param name="path">Путь ссылки</param>
	/// <returns></returns>
	public Task<Guid> GetMeasureIdAsync (string path)
	{
		return Task.FromResult(_storage [path]);
	}

	/// <summary>
	/// Поиск связи (Путь <-> Guid) по маске.
	/// Например, 
	///1) Общие/Температура воздуха/*
	///2) */Температура*
	/// </summary>
	/// <param name="mask">Маска/регулярное выражение ссылок</param>
	/// <returns></returns>
	public Task<IReadOnlyList<KeyValuePair<string, Guid>>> FindLinksByMaskAsync (string mask) 
	{
		return Task.FromResult((IReadOnlyList<KeyValuePair<string, Guid>>) _storage.Where(item => Regex.IsMatch(item.Key, mask)).ToArray());
	}

	/// <summary>
	/// Удалить ссылку на тип измерения по пути
	/// </summary>
	/// <param name="path">Путь/Ссылка</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task RemoveMeasureLinkAsync (string path)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Удалить ссылки на измерения по ид. типа измерения
	/// Внимание, удаляются все ссылки на ид. типа измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task RemoveMeasureLinkAsync (Guid measurementId)
	{
		throw new NotImplementedException();
	}

	private readonly Dictionary<string, Guid> _storage = new()
	{
		{ "Общие/Температура воздуха", Guid.Parse("24FE134B-4CBF-4EB9-A811-2720D4315146") },
		{ "Общие/Входная дверь", Guid.Parse("421673E7-95EF-478C-912A-71F3158FF613") },
		{ "Общие/Вентиляция", Guid.Parse("40EAC794-65E5-432D-84E6-F1B04B14DB8A") },
		{ "Ванная комната/Температура горячей воды", Guid.Parse("462F9446-ADFF-4EA4-8CA1-F1665268520F") },
		{ "Спальня/Температура воздуха", Guid.Parse ("21274707-C7CA-4436-B191-9BAC91C473F5") },
	};
}
