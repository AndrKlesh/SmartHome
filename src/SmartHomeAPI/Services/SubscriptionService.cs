#pragma warning disable CA1515

using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис подписки на измерения
/// Сопоставляет Guid измерения <-> mqtt-топик
/// </summary>
/// <param name="subscriptionRepository">Репозиторий подписок на измерения</param>
public sealed class SubscriptionService (SubscriptionRepository subscriptionRepository, ILogger<SubscriptionService> logger)
{
	/// <summary>
	/// Получить все подписки
	/// </summary>
	/// <returns>Массив всех подписок</returns>
	public async Task<IReadOnlyList<SubscriptionDTO>> GetAllSubscriptionsAsync ()
	{
		List<SubscriptionDomain> subscriptions = await subscriptionRepository.GetAllSubscriptionsAsync().ConfigureAwait(false);
		logger.LogInformation("Получено {Count} подписок", subscriptions.Count);

		return subscriptions.Select(s => new SubscriptionDTO
		{
			MeasurementId = s.MeasurementId,
			Description = s.Description,
			Unit = s.Unit,
			MqttTopic = s.MqttTopic
		}).ToArray();
	}

	/// <summary>
	/// Добавить подписку на mqtt-топик
	/// </summary>
	/// <param name="subscriptionDto"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"/>
	public async Task AddSubscriptionAsync (SubscriptionDTO subscriptionDto)
	{
		ArgumentNullException.ThrowIfNull(subscriptionDto);
		logger.LogInformation("Добавление подписки для измерения '{MeasurementId}' на топик '{MqttTopic}'...", subscriptionDto.MeasurementId, subscriptionDto.MqttTopic);

		SubscriptionDomain subscription = new()
		{
			MeasurementId = subscriptionDto.MeasurementId,
			Description = subscriptionDto.Description,
			Unit = subscriptionDto.Unit,
			MqttTopic = subscriptionDto.MqttTopic,
			ConverterName = "default" // по умолчанию "default"
		};

		await subscriptionRepository.AddSubscriptionAsync(subscription).ConfigureAwait(false);
	}

	/// <summary>
	/// Получить подписку по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <returns>Подписка на mqtt-топик или null, если подписка не найдена</returns>
	public async Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		SubscriptionDomain? subscription = await subscriptionRepository.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);

		if (subscription is null)
		{
			logger.LogWarning("Подписка с ID '{MeasurementId}' не найдена", measurementId);
			return null;
		}

		logger.LogInformation("Подписка найдена: {MqttTopic}", subscription.MqttTopic);
		return new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			Description = subscription.Description,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		};
	}

	/// <summary>
	/// Получить подписку по mqtt-топику
	/// </summary>
	/// <param name="mqttTopic">mqtt-топик</param>
	/// <returns>Подписка на mqtt-топик или null, если подписка не найдена</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task<SubscriptionDTO?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		if (string.IsNullOrWhiteSpace(mqttTopic))
		{
			logger.LogError("Передан пустой MQTT-топик");
			throw new ArgumentNullException(nameof(mqttTopic));
		}

		logger.LogInformation("Поиск подписки по MQTT-топику '{MqttTopic}'...", mqttTopic);
		SubscriptionDomain? subscription = await subscriptionRepository.GetSubscriptionByMqttTopicAsync(mqttTopic).ConfigureAwait(false);

		if (subscription is null)
		{
			logger.LogWarning("Подписка на топик '{MqttTopic}' не найдена", mqttTopic);
			return null;
		}

		logger.LogInformation("Подписка найдена для '{MeasurementId}'", subscription.MeasurementId);
		return new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			Description = subscription.Description,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		};
	}

	/// <summary>
	/// Обновить подписку на mqtt-топик
	/// </summary>
	/// <param name="updatedSubscription">Обновленная подписка</param>
	/// <returns></returns>
	public async Task UpdateSubscriptionAsync (SubscriptionDTO updatedSubscription)
	{
		ArgumentNullException.ThrowIfNull(updatedSubscription);
		logger.LogInformation("Обновление подписки для измерения '{MeasurementId}'...", updatedSubscription.MeasurementId);

		SubscriptionDomain subscription = new()
		{
			MeasurementId = updatedSubscription.MeasurementId,
			Description = updatedSubscription.Description,
			Unit = updatedSubscription.Unit,
			MqttTopic = updatedSubscription.MqttTopic,
			ConverterName = "default"
		};

		await subscriptionRepository.UpdateSubscriptionAsync(subscription).ConfigureAwait(false);
	}

	/// <summary>
	/// Удалить подписку по ид. типа измерения
	/// </summary>
	/// <param name="measurementId"></param>
	/// <returns></returns>
	public async Task DeleteSubscriptionAsync (Guid measurementId)
	{
		logger.LogInformation("Удаление подписки для измерения '{MeasurementId}'...", measurementId);
		await subscriptionRepository.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
	}
}
