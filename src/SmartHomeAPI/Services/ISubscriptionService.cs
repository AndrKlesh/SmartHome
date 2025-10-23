#pragma warning disable CA1515

using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

/// <summary>
/// Интерфейс сервиса подписок на измерения.
/// </summary>
public interface ISubscriptionService
{
	public Task<IReadOnlyList<SubscriptionDTO>> GetAllSubscriptionsAsync ();

	public Task AddSubscriptionAsync (SubscriptionDTO subscriptionDto);

	public Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (Guid measurementId);

	public Task<SubscriptionDTO?> GetSubscriptionByMqttTopicAsync (string mqttTopic);

	public Task UpdateSubscriptionAsync (SubscriptionDTO updatedSubscription);

	public Task DeleteSubscriptionAsync (Guid measurementId);
}
