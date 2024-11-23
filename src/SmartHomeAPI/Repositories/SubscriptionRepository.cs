using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;
public class SubscriptionRepository
{
	private readonly List<SubscriptionDomain> _subscriptions = new();
	private readonly ReaderWriterLockSlim _lock = new();

	public async Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		_lock.EnterReadLock();
		try
		{
			return await Task.FromResult(_subscriptions.ToList());
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public async Task AddSubscriptionAsync (SubscriptionDomain subscription)
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

		await Task.CompletedTask;
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (string measurementId)
	{
		_lock.EnterReadLock();
		try
		{
			return await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId));
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		_lock.EnterReadLock();
		try
		{
			return await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MqttTopic == mqttTopic));
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public async Task UpdateSubscriptionAsync (SubscriptionDomain subscription)
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

		await Task.CompletedTask;
	}

	public async Task DeleteSubscriptionAsync (string measurementId)
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

		await Task.CompletedTask;
	}
}
