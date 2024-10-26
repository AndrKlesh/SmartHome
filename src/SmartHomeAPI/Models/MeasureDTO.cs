namespace SmartHomeAPI.Models;

public class MeasureDTO
{
	public string TopicName { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
}