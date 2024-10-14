namespace SmartHomeWeb.Services;

public class MeasuresStorageService
{
	internal sealed class Measure (string topic, string value, DateTime timestamp)
	{
		public string Topic { get; } = topic;
		public string Value { get; set; } = value;
		public DateTime Timestamp { get; set; } = timestamp;
	}

	internal IEnumerable<Measure> AllMeasures => _measures.Values;

	internal void PutMeasure (string mqttTopic, string value, DateTime timestamp)
	{
		if (_measures.TryGetValue(mqttTopic, out Measure? measure))
		{
			measure.Value = value;
			measure.Timestamp = timestamp;
		}
	}

	private readonly Dictionary<string, Measure> _measures = new()
	{
	  { "home/door", new Measure("home/door", string.Empty, DateTime.MinValue) },
	  { "home/outside/temperature", new Measure("home/outside/temperature", string.Empty, DateTime.MinValue) },
	  { "home/living_room/temperature", new Measure("home/living_room/temperature", string.Empty, DateTime.MinValue) },
	  { "home/living_room/lighting", new Measure("home/living_room/lighting", string.Empty, DateTime.MinValue) },
	  { "home/bathroom/cold_water_temp", new Measure("home/bathroom/cold_water_temp", string.Empty, DateTime.MinValue) },
	  { "home/bathroom/hot_water_temp", new Measure("home/bathroom/hot_water_temp", string.Empty, DateTime.MinValue) },
	  { "home/bathroom/venting", new Measure("home/bathroom/venting", string.Empty, DateTime.MinValue) },
	  { "home/bathroom/air_humidity", new Measure("home/bathroom/air_humidity", string.Empty, DateTime.MinValue) },
	};
}
