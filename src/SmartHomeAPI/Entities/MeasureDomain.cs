#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

public class MeasureDomain
{
	public string MeasurementId { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
}
