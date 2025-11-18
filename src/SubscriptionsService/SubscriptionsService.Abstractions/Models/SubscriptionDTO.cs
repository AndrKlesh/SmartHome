namespace SubscriptionsService.Abstractions.Models;

public sealed class SubscriptionDTO
{
	public Guid MeasurementId { get; set; }
	public string Description { get; set; } = string.Empty;
	public string Unit { get; set; } = string.Empty;
	public string MqttTopic { get; set; } = string.Empty;
}
