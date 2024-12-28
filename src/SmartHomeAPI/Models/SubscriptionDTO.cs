#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

public class SubscriptionDTO
{
	public Guid MeasurementId { get; set; }
	public string MeasurementName { get; set; } = string.Empty;
	public string Unit { get; set; } = string.Empty;
	public string MqttTopic { get; set; } = string.Empty;
}
