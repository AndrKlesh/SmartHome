#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

public class SubscriptionDomain
{
	public Guid MeasurementId { get; set; }
	public string MeasurementName { get; set; } = string.Empty;
	public string Unit { get; set; } = string.Empty;
	public string MqttTopic { get; set; } = string.Empty;
	public string ConverterName { get; set; } = "default";
}
