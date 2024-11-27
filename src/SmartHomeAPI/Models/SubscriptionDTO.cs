namespace SmartHomeAPI.Models;

internal class SubscriptionDTO
{
	internal string MeasurementId { get; set; } = string.Empty;
	internal string MeasurementName { get; set; } = string.Empty;
	internal string Unit { get; set; } = string.Empty;
	internal string MqttTopic { get; set; } = string.Empty;
}
