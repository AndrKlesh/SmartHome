#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public class SubscriptionRepository
{
	//TODO: Убрать заглушки подписок
	private readonly List<SubscriptionDomain> _subscriptions =
	[
		new SubscriptionDomain()
		{
			MeasurementId = Guid.Parse("462F9446-ADFF-4EA4-8CA1-F1665268520F"),
			Description = "Температура горячей воды",
			Unit = "°C",
			MqttTopic = "home/bathroom/hot_water_temp",
			ConverterName = "default",
		},
		new SubscriptionDomain()
		{
			MeasurementId = Guid.Parse("21274707-C7CA-4436-B191-9BAC91C473F5"),
			Description = "Температура в помещении",
			Unit = "°C",
			MqttTopic = "home/living_room/temperature",
			ConverterName = "default",
		},
		new SubscriptionDomain()
		{
			MeasurementId = Guid.Parse("24FE134B-4CBF-4EB9-A811-2720D4315146"),
			Description = "Температура воздуха снаружи здания",
			Unit = "°C",
			MqttTopic = "home/outside/temperature",
			ConverterName = "default",
		},
		new SubscriptionDomain()
		{
			MeasurementId = Guid.Parse("421673E7-95EF-478C-912A-71F3158FF613"),
			Description = "Входная дверь",
			Unit = "",
			MqttTopic = "home/door",
			ConverterName = "default",
		},
		new SubscriptionDomain()
		{
			MeasurementId = Guid.Parse("40EAC794-65E5-432D-84E6-F1B04B14DB8A"),
			Description = "Вентиляция",
			Unit = "",
			MqttTopic = "home/venting",
			ConverterName = "default",
		},
	];
	private readonly ReaderWriterLockSlim _lock = new();

	internal async Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		_lock.EnterReadLock();
		try
		{
			return await Task.FromResult(_subscriptions.ToList()).ConfigureAwait(false);
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	internal async Task AddSubscriptionAsync (SubscriptionDomain subscription)
	{
		_lock.EnterWriteLock();
		try
		{
			_subscriptions.Add(subscription);
		}
		finally
		{
			_lock.ExitWriteLock();
		}

		await Task.CompletedTask.ConfigureAwait(false);
	}

	internal async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		_lock.EnterReadLock();
		try
		{
			return await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId)).ConfigureAwait(false);
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	internal async Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		_lock.EnterReadLock();
		try
		{
			return await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MqttTopic == mqttTopic)).ConfigureAwait(false);
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	internal async Task UpdateSubscriptionAsync (SubscriptionDomain subscription)
	{
		_lock.EnterUpgradeableReadLock();
		try
		{
			SubscriptionDomain? existingSubscription = _subscriptions.FirstOrDefault(s => s.MeasurementId == subscription.MeasurementId);
			if (existingSubscription != null)
			{
				_lock.EnterWriteLock();
				try
				{
					existingSubscription.Description = subscription.Description;
					existingSubscription.Unit = subscription.Unit;
					existingSubscription.MqttTopic = subscription.MqttTopic;
					existingSubscription.ConverterName = "default";
				}
				finally
				{
					_lock.ExitWriteLock();
				}
			}
		}
		finally
		{
			_lock.ExitUpgradeableReadLock();
		}

		await Task.CompletedTask.ConfigureAwait(false);
	}

	internal async Task DeleteSubscriptionAsync (Guid measurementId)
	{
		_lock.EnterUpgradeableReadLock();
		try
		{
			SubscriptionDomain? subscription = _subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId);
			if (subscription != null)
			{
				_lock.EnterWriteLock();
				try
				{
					_ = _subscriptions.Remove(subscription);
				}
				finally
				{
					_lock.ExitWriteLock();
				}
			}
		}
		finally
		{
			_lock.ExitUpgradeableReadLock();
		}

		await Task.CompletedTask.ConfigureAwait(false);
	}
}
