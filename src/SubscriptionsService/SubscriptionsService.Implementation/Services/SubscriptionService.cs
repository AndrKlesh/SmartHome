using Microsoft.Extensions.Logging;
using SubscriptionsService.Abstractions.Entities;
using SubscriptionsService.Abstractions.Models;
using SubscriptionsService.Abstractions.Repositories;
using SubscriptionsService.Abstractions.Services;

namespace SubscriptionsService.Implementation.Services;

public sealed class SubscriptionService : ISubscriptionService
{
	private readonly ISubscriptionRepository _repository;
	private readonly ILogger<SubscriptionService> _logger;

	public SubscriptionService (ISubscriptionRepository repository, ILogger<SubscriptionService> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<IReadOnlyList<SubscriptionDTO>> GetAllSubscriptionsAsync ()
	{
		List<SubscriptionDomain> list = await _repository.GetAllSubscriptionsAsync().ConfigureAwait(false);
		_logger.LogInformation("Service: Найдено {Count} подписок", list.Count);
		return list.Select(s => new SubscriptionDTO
		{
			MeasurementId = s.MeasurementId,
			Description = s.Description,
			Unit = s.Unit,
			MqttTopic = s.MqttTopic
		}).ToArray();
	}

	public async Task<SubscriptionDTO?> GetSubscriptionByMeasurementIdAsync (Guid measurementId)
	{
		SubscriptionDomain s = await _repository.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		if (s == null)
		{
			_logger.LogWarning("Service: подписка {Id} не найдена", measurementId);
			return null;
		}
		return new SubscriptionDTO
		{
			MeasurementId = s.MeasurementId,
			Description = s.Description,
			Unit = s.Unit,
			MqttTopic = s.MqttTopic
		};
	}

	public async Task<SubscriptionDTO?> GetSubscriptionByMqttTopicAsync (string mqttTopic)
	{
		if (string.IsNullOrWhiteSpace(mqttTopic))
		{
			throw new ArgumentNullException(nameof(mqttTopic));
		}

		SubscriptionDomain s = await _repository.GetSubscriptionByMqttTopicAsync(mqttTopic).ConfigureAwait(false);
		if (s == null)
		{
			_logger.LogWarning("Service: подписка для топика {Topic} не найдена", mqttTopic);
			return null;
		}
		return new SubscriptionDTO
		{
			MeasurementId = s.MeasurementId,
			Description = s.Description,
			Unit = s.Unit,
			MqttTopic = s.MqttTopic
		};
	}

	public async Task AddSubscriptionAsync (SubscriptionDTO subscription)
	{
		ArgumentNullException.ThrowIfNull(subscription);
		_logger.LogInformation("Service: добавление подписки для измерения {Id} для топика {Topic}", subscription.MeasurementId, subscription.MqttTopic);
		SubscriptionDomain domain = new SubscriptionDomain
		{
			MeasurementId = subscription.MeasurementId,
			Description = subscription.Description,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic,
			ConverterName = "default"
		};
		await _repository.AddSubscriptionAsync(domain).ConfigureAwait(false);
	}

	public async Task UpdateSubscriptionAsync (SubscriptionDTO subscription)
	{
		ArgumentNullException.ThrowIfNull(subscription);
		_logger.LogInformation("Service: обновление подписки {Id}", subscription.MeasurementId);
		SubscriptionDomain domain = new SubscriptionDomain
		{
			MeasurementId = subscription.MeasurementId,
			Description = subscription.Description,
			Unit = subscription.Unit,
			MqttTopic = subscription.MqttTopic,
			ConverterName = "default"
		};
		await _repository.UpdateSubscriptionAsync(domain).ConfigureAwait(false);
	}

	public async Task DeleteSubscriptionAsync (Guid measurementId)
	{
		_logger.LogInformation("Service: удаление подписки {Id}", measurementId);
		await _repository.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
	}
}
