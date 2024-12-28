#pragma warning disable CA1515

using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

public class SubscriptionService (SubscriptionRepository subscriptionRepository)
{
	private readonly SubscriptionRepository _subscriptionRepository = subscriptionRepository;

	public async Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync ()
	{
		List<SubscriptionDomain> subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync().ConfigureAwait(false);
		return subscriptions.Select(s => new SubscriptionDTO
		{
			MeasurementId = s.MeasurementId,
			MeasurementName = s.MeasurementName,
			Unit = s.Unit,
			MqttTopic = s.MqttTopic
		}).ToList();
	}

	public async Task AddSubscriptionAsync (SubscriptionDTO subscriptionDto)
	{
		SubscriptionDomain subscription = new()
		{
			MeasurementId = subscriptionDto.MeasurementId,
			MeasurementName = subscriptionDto.MeasurementName,
			Unit = subscriptionDto.Unit,
			MqttTopic = subscriptionDto.MqttTopic,
			ConverterName = "default" // по умолчанию "default"
		};

		await _subscriptionRepository.AddSubscriptionAsync(subscription).ConfigureAwait(false);
	}

	public async Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		return subscription != null ? new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			MeasurementName = subscription.MeasurementName,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		} : null;
	}

	public async Task<SubscriptionDTO?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMqttTopicAsync(mqttTopic).ConfigureAwait(false);
		return subscription != null ? new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			MeasurementName = subscription.MeasurementName,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		} : null;
	}

	public async Task UpdateSubscriptionAsync (Guid measurementId, SubscriptionDTO updatedSubscription)
	{
		SubscriptionDomain subscription = new()
		{
			MeasurementId = measurementId,
			MeasurementName = updatedSubscription.MeasurementName,
			Unit = updatedSubscription.Unit,
			MqttTopic = updatedSubscription.MqttTopic,
			ConverterName = "default"
		};

		await _subscriptionRepository.UpdateSubscriptionAsync(subscription).ConfigureAwait(false);
	}

	public async Task DeleteSubscriptionAsync (Guid measurementId)
	{
		await _subscriptionRepository.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
	}
}
