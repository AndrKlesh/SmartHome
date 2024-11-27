#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

public class MeasureDomain
{
	public Guid Id { get; set; }
	public Guid TopicId { get; set; }
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
	public required TopicDomain Topic { get; set; }
}
