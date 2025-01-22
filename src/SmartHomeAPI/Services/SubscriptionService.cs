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
public class SubscriptionService (SubscriptionRepository subscriptionRepository)
{
	/// <summary>
	/// Получить все подписки
	/// </summary>
	/// <returns>Массив всех подписок</returns>
	public async Task<IReadOnlyList<SubscriptionDTO>> GetAllSubscriptionsAsync ()
	{
		List<SubscriptionDomain> subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync().ConfigureAwait(false);
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

		SubscriptionDomain subscription = new()
		{
			MeasurementId = subscriptionDto.MeasurementId,
			Description = subscriptionDto.Description,
			Unit = subscriptionDto.Unit,
			MqttTopic = subscriptionDto.MqttTopic,
			ConverterName = "default" // по умолчанию "default"
		};

		await _subscriptionRepository.AddSubscriptionAsync(subscription).ConfigureAwait(false);
	}

	/// <summary>
	/// Получить подписку по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <returns>Подписка на mqtt-топик или null, если подписка не найдена</returns>
	public async Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		return subscription != null ? new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			Description = subscription.Description,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		} : null;
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
			throw new ArgumentNullException(nameof(mqttTopic));
		}

		SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMqttTopicAsync(mqttTopic).ConfigureAwait(false);
		return subscription != null ? new SubscriptionDTO
		{
			MeasurementId = subscription.MeasurementId,
			Description = subscription.Description,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic
		} : null;
	}

	/// <summary>
	/// Обновить подписку на mqtt-топик
	/// </summary>
	/// <param name="updatedSubscription">Обновленная подписка</param>
	/// <returns></returns>
	public async Task UpdateSubscriptionAsync (SubscriptionDTO updatedSubscription)
	{
		ArgumentNullException.ThrowIfNull(updatedSubscription);
		SubscriptionDomain subscription = new()
		{
			MeasurementId = updatedSubscription.MeasurementId,
			Description = updatedSubscription.Description,
			Unit = updatedSubscription.Unit,
			MqttTopic = updatedSubscription.MqttTopic,
			ConverterName = "default"
		};

		await _subscriptionRepository.UpdateSubscriptionAsync(subscription).ConfigureAwait(false);
	}

	/// <summary>
	/// Удалить подписку по ид. типа измерения
	/// </summary>
	/// <param name="measurementId"></param>
	/// <returns></returns>
	public async Task DeleteSubscriptionAsync (Guid measurementId)
	{
		await _subscriptionRepository.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
	}

	private readonly SubscriptionRepository _subscriptionRepository = subscriptionRepository;
}
