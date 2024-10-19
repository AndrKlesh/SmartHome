namespace SmartHomeAPI.Entities;

public class MeasureDomain
{
	public int Id { get; set; }
	public int TopicId { get; set; }
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }

	public required TopicDomain Topic { get; set; }
}
