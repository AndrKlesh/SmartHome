#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public sealed class SubscriptionRepository (ILogger<SubscriptionRepository> logger) : IDisposable
{
	private readonly ReaderWriterLockSlim _lock = new();
	private bool _disposed;

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

	internal async Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		logger.LogInformation("Получение всех подписок");

		_lock.EnterReadLock();
		try
		{
			List<SubscriptionDomain> subscriptions = await Task.FromResult(_subscriptions.ToList()).ConfigureAwait(false);
			logger.LogInformation("Найдено {Count} подписок", subscriptions.Count);
			return subscriptions;
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	internal async Task AddSubscriptionAsync (SubscriptionDomain subscription)
	{
		logger.LogInformation("Добавление подписки для измерения с ID {MeasurementId}", subscription.MeasurementId);

		_lock.EnterWriteLock();
		try
		{
			_subscriptions.Add(subscription);
			logger.LogInformation("Подписка для измерения с ID {MeasurementId} успешно добавлена", subscription.MeasurementId);
		}
		finally
		{
			_lock.ExitWriteLock();
		}

		await Task.CompletedTask.ConfigureAwait(false);
	}

	internal async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		logger.LogInformation("Получение подписки для измерения с ID {MeasurementId}", measurementId);

		_lock.EnterReadLock();
		try
		{
			SubscriptionDomain subscription = await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId)).ConfigureAwait(false);
			if (subscription != null)
			{
				logger.LogInformation("Найдена подписка для измерения с ID {MeasurementId}", measurementId);
			}
			else
			{
				logger.LogWarning("Подписка для измерения с ID {MeasurementId} не найдена", measurementId);
			}

			return subscription;
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	internal async Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		logger.LogInformation("Получение подписки для MQTT топика {MqttTopic}", mqttTopic);

		_lock.EnterReadLock();
		try
		{
			SubscriptionDomain subscription = await Task.FromResult(_subscriptions.FirstOrDefault(s => s.MqttTopic == mqttTopic)).ConfigureAwait(false);
			if (subscription != null)
			{
				logger.LogInformation("Найдена подписка для MQTT топика {MqttTopic}", mqttTopic);
			}
			else
			{
				logger.LogWarning("Подписка для MQTT топика {MqttTopic} не найдена", mqttTopic);
			}

			return subscription;
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	internal async Task UpdateSubscriptionAsync (SubscriptionDomain subscription)
	{
		logger.LogInformation("Обновление подписки для измерения с ID {MeasurementId}", subscription.MeasurementId);

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
					logger.LogInformation("Подписка для измерения с ID {MeasurementId} успешно обновлена", subscription.MeasurementId);
				}
				finally
				{
					_lock.ExitWriteLock();
				}
			}
			else
			{
				logger.LogWarning("Подписка для измерения с ID {MeasurementId} не найдена для обновления", subscription.MeasurementId);
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
		logger.LogInformation("Удаление подписки для измерения с ID {MeasurementId}", measurementId);

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
					logger.LogInformation("Подписка для измерения с ID {MeasurementId} успешно удалена", measurementId);
				}
				finally
				{
					_lock.ExitWriteLock();
				}
			}
			else
			{
				logger.LogWarning("Подписка для измерения с ID {MeasurementId} не найдена для удаления", measurementId);
			}
		}
		finally
		{
			_lock.ExitUpgradeableReadLock();
		}

		await Task.CompletedTask.ConfigureAwait(false);
	}

	public void Dispose ()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	internal void Dispose (bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			_lock.Dispose();
		}

		_disposed = true;
	}
}
