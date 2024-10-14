namespace SmartHomeWeb.Services;

public class MeasuresUIPreprocessingService (IMeasuresStorageService measuresStorageService)
{
	private readonly IMeasuresStorageService _measuresStorageService = measuresStorageService;

	public IEnumerable<string> EnumerateAllMeasures ()
	{
		IEnumerable<Models.Measure> allMeasures = _measuresStorageService.GetAllMeasures()
								.GroupBy(m => m.Topic)
								.Select(g => g.Last());

		return allMeasures.Select(m => $"<p>{_names [m.Topic]}</p><p>{m.Value} {_units [m.Topic]}</p>");
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
