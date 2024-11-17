using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

public class SubscriptionsService
{
	private readonly List<SubscriptionDomain> _subscriptions = new();

	public async Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		return await Task.FromResult(_subscriptions);
	}

	public async Task AddSubscriptionAsync (SubscriptionDTO subscriptionDto)
	{
		SubscriptionDomain subscription = new()
		{
			MeasurementId = subscriptionDto.MeasurementId,
			MeasurementName = subscriptionDto.MeasurementName,
			Unit = subscriptionDto.Unit,
			MqttTopic = subscriptionDto.MqttTopic,
			ConverterName = subscriptionDto.ConverterName
		};

		_subscriptions.Add(subscription);
		await Task.CompletedTask;
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (string measurementId)
	{
		return await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId));
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		return await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MqttTopic == mqttTopic));
	}

	public async Task UpdateSubscriptionAsync (string measurementId, SubscriptionDTO updatedSubscription)
	{
		SubscriptionDomain? existingSubscription = _subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId);
		if (existingSubscription == null)
		{
			throw new ArgumentException($"Subscription with measurement ID '{measurementId}' not found.");
		}

		existingSubscription.MeasurementName = updatedSubscription.MeasurementName;
		existingSubscription.Unit = updatedSubscription.Unit;
		existingSubscription.MqttTopic = updatedSubscription.MqttTopic;
		existingSubscription.ConverterName = updatedSubscription.ConverterName;

		await Task.CompletedTask;
	}

	public async Task DeleteSubscriptionAsync (string measurementId)
	{
		SubscriptionDomain? subscription = _subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId);
		if (subscription == null)
		{
			throw new ArgumentException($"Subscription with measurement ID '{measurementId}' not found.");
		}

		_ = _subscriptions.Remove(subscription);
		await Task.CompletedTask;
	}
}
