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
			MeasurementId = "home/bathroom/hot_water_temp",
			MeasurementName = "Температура горячей воды",
			Unit = "°C",
			MqttTopic = "home/bathroom/hot_water_temp",
			ConverterName = "default",
		},
		new SubscriptionDomain()
		{
			MeasurementId = "home/living_room/temperature",
			MeasurementName = "Температура в помещении",
			Unit = "°C",
			MqttTopic = "home/living_room/temperature",
			ConverterName = "default",
		},
		new SubscriptionDomain()
		{
			MeasurementId = "home/outside/temperature",
			MeasurementName = "Температура воздуха снаружи здания",
			Unit = "°C",
			MqttTopic = "home/outside/temperature",
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

	internal async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (string measurementId)
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
					existingSubscription.MeasurementName = subscription.MeasurementName;
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

	internal async Task DeleteSubscriptionAsync (string measurementId)
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
