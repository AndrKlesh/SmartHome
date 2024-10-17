namespace SmartHomeWeb.Services;

public class MeasuresUIPreprocessingService (IMeasuresStorageService measuresStorageService)
{
	private readonly IMeasuresStorageService _measuresStorageService = measuresStorageService;

	// Если measure отсутствует или нет данных в _names или _units, возвращаем сообщение об отсутствии данных.
	internal IEnumerable<string> EnumerateAllMeasures ()
	{
		List<string> result = new();

		// Получаем все доступные ID
		foreach (string id in _names.Keys)
		{
			Models.Measure? measure = _measuresStorageService.GetLastMeasureById(id);

			// Проверяем, что измерение существует и в словарях есть записи для текущего ID
			if (measure != null && _units.TryGetValue(id, out string? unit))
			{
				string name = _names [id];

				result.Add($"<p>{name}</p><p>{measure.Value} {unit}</p>");
			}
			else
			{
				// Если данные отсутствуют, добавляем сообщение об отсутствии данных
				result.Add($"<p>Неизвестный топик: {id}</p><p>Нет доступных измерений</p>");
			}
		}

		return result;
	}

	// Альтернативный подход: Исключение отсутствующих данных
	internal IEnumerable<string> EnumerateAllMeasuresAlternative ()
	{
		List<string> result = new();

		foreach (string id in _names.Keys)
		{
			Models.Measure? measure = _measuresStorageService.GetLastMeasureById(id);

			// Проверяем, что измерение существует и в словарях есть записи для текущего ID
			if (measure != null && _names.TryGetValue(id, out string? name) && _units.TryGetValue(id, out string? value))
			{
				result.Add($"<p>{name}</p><p>{measure.Value} {value}</p>");
			}
		}

		return result;
	}

	// TODO: Перенести строки в ресурсы для локализации
	private readonly Dictionary<string, string> _names = new()
	{
		{ "home/door", "Входная дверь" },
		{ "home/outside/temperature", "Температура на улице" },
		{ "home/living_room/temperature", "Температура в комнате" },
		{ "home/living_room/lighting", "Освещение в комнате" },
		{ "home/bathroom/cold_water_temp", "Температура холодной воды" },
		{ "home/bathroom/hot_water_temp", "Температура горячей воды" },
		{ "home/bathroom/venting", "Вентиляция ванной комнаты" },
		{ "home/bathroom/air_humidity", "Влажность воздуха ванной комнаты" }
	};

	private readonly Dictionary<string, string> _units = new()
	{
		{ "home/door", string.Empty },
		{ "home/outside/temperature", "°C" },
		{ "home/living_room/temperature", "°C" },
		{ "home/living_room/lighting", string.Empty },
		{ "home/bathroom/cold_water_temp", "°C" },
		{ "home/bathroom/hot_water_temp", "°C" },
		{ "home/bathroom/venting", string.Empty },
		{ "home/bathroom/air_humidity", "%" }
	};
}
