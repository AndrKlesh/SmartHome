namespace SmartHomeWeb.Models;

public class Measure (string topic, string value, DateTime timestamp)
{
	public string Topic { get; } = topic;
	public string Value { get; set; } = value;
	public DateTime Timestamp { get; set; } = timestamp;
}
