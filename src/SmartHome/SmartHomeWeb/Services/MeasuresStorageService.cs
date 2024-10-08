namespace SmartHomeWeb.Services
{
  public class MeasuresStorageService
  {
    public class Measure
    {
      public Measure(string topic, string value, DateTime timestamp)
      {
        Topic = topic;
        Value = value;
        Timestamp = timestamp;
      }

      public string Topic { get; } = string.Empty;
      public string Value { get; set; } = string.Empty;
      public DateTime Timestamp { get; set; }
    }

    public IEnumerable<Measure> GetAllMeasures() 
    {
      return _measures.Values;
    }

    public void PutMeasure(string mqttTopic, string value, DateTime timestamp) 
    {
      if (_measures.TryGetValue(mqttTopic, out var measure)) 
      {
        measure.Value = value;
        measure.Timestamp = timestamp;
      }
    }

    private IReadOnlyDictionary<string, Measure> _measures = new Dictionary<string, Measure>() 
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
}
