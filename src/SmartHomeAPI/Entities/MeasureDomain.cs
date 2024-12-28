#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

public class MeasureDomain
{
	public Guid MeasurementId { get; set; }
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
}
