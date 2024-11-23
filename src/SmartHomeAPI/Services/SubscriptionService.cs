using System.Collections.Concurrent;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

public class SubscriptionsService
{
	private readonly ConcurrentDictionary<string, SubscriptionDomain> _subscriptions = new();

	public async Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		return await Task.FromResult(_subscriptions.Values.ToList());
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

		if (!_subscriptions.TryAdd(subscription.MeasurementId, subscription))
		{
			throw new InvalidOperationException($"Subscription with ID '{subscription.MeasurementId}' already exists");
		}

		await Task.CompletedTask;
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (string measurementId)
	{
		_ = _subscriptions.TryGetValue(measurementId, out SubscriptionDomain? subscription);
		return await Task.FromResult(subscription);
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		SubscriptionDomain? subscription = _subscriptions.Values.FirstOrDefault(s => s.MqttTopic == mqttTopic);
		return await Task.FromResult(subscription);
	}

	public async Task UpdateSubscriptionAsync (string measurementId, SubscriptionDTO updatedSubscription)
	{
		if (!_subscriptions.TryGetValue(measurementId, out SubscriptionDomain? existingSubscription))
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
		if (!_subscriptions.TryRemove(measurementId, out _))
		{
			throw new ArgumentException($"Subscription with measurement ID '{measurementId}' not found.");
		}

		await Task.CompletedTask;
	}
}
