#pragma warning disable CA1515

using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public sealed class SubscriptionRepository
{
	private readonly ILogger<SubscriptionRepository> logger;
	private readonly IOptionsMonitor<List<SubscriptionDomain>> optionsMonitor;
	private ConcurrentBag<SubscriptionDomain> subscriptions;

	public SubscriptionRepository (ILogger<SubscriptionRepository> logger, IOptionsMonitor<List<SubscriptionDomain>> optionsMonitor)
	{
		this.logger = logger;
		this.optionsMonitor = optionsMonitor;

		subscriptions = new ConcurrentBag<SubscriptionDomain>(optionsMonitor?.CurrentValue);

		_ = optionsMonitor.OnChange(newSubscriptions => subscriptions = new ConcurrentBag<SubscriptionDomain>(newSubscriptions));
	}

	public async Task<List<SubscriptionDomain>> GetAllSubscriptionsAsync ()
	{
		return await Task.FromResult(subscriptions.ToList()).ConfigureAwait(false);
	}

	public async Task<SubscriptionDomain?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		logger.LogInformation("Получение подписки для измерения с ID '{MeasurementId}'...", measurementId);

		SubscriptionDomain subscription = subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId);
		if (subscription == null)
		{
			logger.LogWarning("Подписка для измерения с ID '{MeasurementId}' не найдена", measurementId);
		}
		else
		{
			logger.LogInformation("Найдена подписка для измерения с ID '{MeasurementId}'", measurementId);
		}

		return await Task.FromResult(subscription).ConfigureAwait(false);
	}

	internal async Task<SubscriptionDomain?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		logger.LogInformation("Получение подписки для MQTT топика '{MqttTopic}'...", mqttTopic);

		SubscriptionDomain subscription = subscriptions.FirstOrDefault(s => s.MqttTopic == mqttTopic);
		if (subscription == null)
		{
			logger.LogWarning("Подписка для MQTT топика '{MqttTopic}' не найдена", mqttTopic);
		}
		else
		{
			logger.LogInformation("Найдена подписка для MQTT топика '{MqttTopic}'", mqttTopic);
		}

		return await Task.FromResult(subscription).ConfigureAwait(false);
	}

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
	public async Task AddSubscriptionAsync (SubscriptionDomain subscription)
	{
		throw new NotImplementedException("Добавление подписки для измерения не реализовано");
		/*
		logger.LogInformation("Добавление подписки для измерения с ID '{MeasurementId}'...", subscription.MeasurementId);
		subscriptions.Add(subscription);

		logger.LogInformation("Подписка для измерения с ID '{MeasurementId}' добавлена", subscription.MeasurementId);
		await Task.CompletedTask.ConfigureAwait(false);
		*/
	}

	public async Task DeleteSubscriptionAsync (Guid measurementId)
	{
		throw new NotImplementedException("Удаление подписки для измерения не реализовано");

		/*
		logger.LogInformation("Удаление подписки для измерения с ID '{MeasurementId}'...", measurementId);

		SubscriptionDomain subscription = subscriptions.FirstOrDefault(s => s.MeasurementId == measurementId);
		if (subscription != null)
		{
			subscriptions = new ConcurrentBag<SubscriptionDomain>(subscriptions.Where(s => s.MeasurementId != measurementId));

			logger.LogInformation("Подписка для измерения с ID '{MeasurementId}' удалена", measurementId);
		}
		else
		{
			logger.LogWarning("Подписка для измерения с ID '{MeasurementId}' не найдена для удаления", measurementId);
		}

		await Task.CompletedTask.ConfigureAwait(false);
		*/
	}

	public async Task UpdateSubscriptionAsync (SubscriptionDomain subscription)
	{
		throw new NotImplementedException("Обновление подписки для измерения не реализовано");

		/*
		logger.LogInformation("Обновление подписки для измерения с ID '{MeasurementId}'...", subscription.MeasurementId);

		SubscriptionDomain existingSubscription = subscriptions.FirstOrDefault(s => s.MeasurementId == subscription.MeasurementId);
		if (existingSubscription != null)
		{
			subscriptions = new ConcurrentBag<SubscriptionDomain>(subscriptions.Where(s => s.MeasurementId != subscription.MeasurementId));
			subscriptions.Add(subscription);

			logger.LogInformation("Подписка для измерения с ID '{MeasurementId}' обновлена", subscription.MeasurementId);
		}
		else
		{
			logger.LogWarning("Подписка для измерения с ID '{MeasurementId}' не найдена для обновления", subscription.MeasurementId);
		}

		await Task.CompletedTask.ConfigureAwait(false);
		*/
	}
}
