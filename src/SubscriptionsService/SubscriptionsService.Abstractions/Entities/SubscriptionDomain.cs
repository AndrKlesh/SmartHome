namespace SubscriptionsService.Abstractions.Entities;

public sealed class SubscriptionDomain
{
	public Guid MeasurementId { get; set; }
	public string Description { get; set; } = string.Empty;
	public string Unit { get; set; } = string.Empty;
	public string MqttTopic { get; set; } = string.Empty;
	public string ConverterName { get; set; } = "default";
}
