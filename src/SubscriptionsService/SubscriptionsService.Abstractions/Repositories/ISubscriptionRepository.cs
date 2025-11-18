using SubscriptionsService.Abstractions.Entities;

namespace SubscriptionsService.Abstractions.Repositories;

public interface ISubscriptionRepository
{
	public Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ();
	public Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (Guid measurementId);
	public Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic);

	public Task AddSubscriptionAsync (SubscriptionDomain subscription);
	public Task UpdateSubscriptionAsync (SubscriptionDomain subscription);
	public Task DeleteSubscriptionAsync (Guid measurementId);
}
