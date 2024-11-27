using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

internal class SubscriptionService (SubscriptionRepository subscriptionRepository)
{
	private readonly SubscriptionRepository _subscriptionRepository = subscriptionRepository;

	internal async Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync ()
	{
		List<SubscriptionDomain> subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync();
		return subscriptions.Select(s => new SubscriptionDTO
		{
			MeasurementId = s.MeasurementId,
			MeasurementName = s.MeasurementName,
			Unit = s.Unit,
			MqttTopic = s.MqttTopic
		}).ToList();
	}

	internal async Task AddSubscriptionAsync (SubscriptionDTO subscriptionDto)
	{
		SubscriptionDomain subscription = new()
		{
			MeasurementId = subscriptionDto.MeasurementId,
			MeasurementName = subscriptionDto.MeasurementName,
			Unit = subscriptionDto.Unit,
			MqttTopic = subscriptionDto.MqttTopic,
			ConverterName = "default" // по умолчанию "default"
		};

		await _subscriptionRepository.AddSubscriptionAsync(subscription);
	}

	internal async Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (string measurementId)
	{
		SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMeasurementIdAsync(measurementId);
		return subscription != null ? new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			MeasurementName = subscription.MeasurementName,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		} : null;
	}

	internal async Task<SubscriptionDTO?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMqttTopicAsync(mqttTopic);
		return subscription != null ? new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			MeasurementName = subscription.MeasurementName,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		} : null;
	}

	internal async Task UpdateSubscriptionAsync (string measurementId, SubscriptionDTO updatedSubscription)
	{
		SubscriptionDomain subscription = new()
		{
			MeasurementId = measurementId,
			MeasurementName = updatedSubscription.MeasurementName,
			Unit = updatedSubscription.Unit,
			MqttTopic = updatedSubscription.MqttTopic,
			ConverterName = "default"
		};

		await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
	}

	internal async Task DeleteSubscriptionAsync (string measurementId)
	{
		await _subscriptionRepository.DeleteSubscriptionAsync(measurementId);
	}
}
