using SubscriptionsService.Abstractions.Models;

namespace SubscriptionsService.Abstractions.Services;

public interface ISubscriptionService
{
	public Task<IReadOnlyList<SubscriptionDTO>> GetAllSubscriptionsAsync ();
	public Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (Guid measurementId);
	public Task<SubscriptionDTO?> GetSubscriptionByMqttTopicAsync (string mqttTopic);

	public Task AddSubscriptionAsync (SubscriptionDTO subscription);
	public Task UpdateSubscriptionAsync (SubscriptionDTO subscription);
	public Task DeleteSubscriptionAsync (Guid measurementId);
}
