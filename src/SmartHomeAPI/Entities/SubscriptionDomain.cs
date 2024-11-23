namespace SmartHomeAPI.Entities;

public class SubscriptionDomain
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string MeasurementId { get; set; } = string.Empty;
	public string MeasurementName { get; set; } = string.Empty;
	public string Unit { get; set; } = string.Empty;
	public string MqttTopic { get; set; } = string.Empty;
	public string ConverterName { get; set; } = "default";
}