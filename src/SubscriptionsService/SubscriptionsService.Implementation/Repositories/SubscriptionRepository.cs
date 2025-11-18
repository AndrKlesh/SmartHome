using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubscriptionsService.Abstractions.Entities;
using SubscriptionsService.Abstractions.Repositories;

namespace SubscriptionsService.Implementation.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
	private readonly ILogger<SubscriptionRepository> _logger;
	private readonly IOptionsMonitor<List<SubscriptionDomain>> _optionsMonitor;

	private readonly ConcurrentDictionary<Guid, SubscriptionDomain> _store = new();

	public SubscriptionRepository (ILogger<SubscriptionRepository> logger, IOptionsMonitor<List<SubscriptionDomain>> optionsMonitor)
	{
		_logger = logger;
		_optionsMonitor = optionsMonitor;

		// Seed from configuration if provided
		List<SubscriptionDomain>? initial = optionsMonitor.CurrentValue;
		if (initial != null)
		{
			foreach (SubscriptionDomain s in initial)
			{
				_store [s.MeasurementId] = s;
			}
		}

		// If configuration changes, re-seed (but do not override runtime mutations)
		_ = optionsMonitor.OnChange(updated =>
		{
			if (updated is null)
				return;
			foreach (SubscriptionDomain s in updated)
			{
				_store.AddOrUpdate(s.MeasurementId, s, (_, __) => s);
			}
		});
	}

	public Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		return Task.FromResult(_store.Values.ToList());
	}

	public Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		_logger.LogInformation("Repository: GetSubscriptionByMeasurementId {Id}", measurementId);
		_store.TryGetValue(measurementId, out SubscriptionDomain sub);
		return Task.FromResult(sub);
	}

	public Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		_logger.LogInformation("Repository: GetSubscriptionByMqttTopic {Topic}", mqttTopic);
		SubscriptionDomain sub = _store.Values.FirstOrDefault(s => string.Equals(s.MqttTopic, mqttTopic, StringComparison.OrdinalIgnoreCase));
		return Task.FromResult(sub);
	}

	public Task AddSubscriptionAsync (SubscriptionDomain subscription)
	{
		if (subscription == null)
			throw new ArgumentNullException(nameof(subscription));
		_logger.LogInformation("Repository: AddSubscription {Id}", subscription.MeasurementId);
		if (!_store.TryAdd(subscription.MeasurementId, subscription))
		{
			_logger.LogWarning("Repository: subscription with id {Id} already exists — will overwrite", subscription.MeasurementId);
			_store [subscription.MeasurementId] = subscription;
		}
		return Task.CompletedTask;
	}

	public Task UpdateSubscriptionAsync (SubscriptionDomain subscription)
	{
		if (subscription == null)
			throw new ArgumentNullException(nameof(subscription));
		_logger.LogInformation("Repository: UpdateSubscription {Id}", subscription.MeasurementId);
		_store.AddOrUpdate(subscription.MeasurementId, subscription, (_, __) => subscription);
		return Task.CompletedTask;
	}

	public Task DeleteSubscriptionAsync (Guid measurementId)
	{
		_logger.LogInformation("Repository: DeleteSubscription {Id}", measurementId);
		_store.TryRemove(measurementId, out _);
		return Task.CompletedTask;
	}
}
