#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// Контроллер подписки на mqtt-топики
/// </summary>
/// <param name="subscriptionsService"></param>
[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController (SubscriptionService subscriptionsService, ILogger<SubscriptionsController> logger) : Controller
{
	/// <summary>
	/// Получить все подписки
	/// </summary>
	/// <returns></returns>
	[HttpGet("getAllSubscriptions")]
	public async Task<ActionResult<IReadOnlyList<SubscriptionDomain>>> GetAllSubscriptions ()
	{
		logger.LogInformation("Запрос всех подписок...");
		IReadOnlyList<SubscriptionDTO> subscriptions = await subscriptionsService.GetAllSubscriptionsAsync().ConfigureAwait(false);

		if (subscriptions.Count == 0)
		{
			logger.LogWarning("Список подписок пуст");
			return NotFound(new { message = "Список подписок пуст" });
		}
		else
		{
			logger.LogInformation("Найдено {Count} подписок", subscriptions.Count);
			return Ok(subscriptions);
		}
	}

	/// <summary>
	/// Получить подписку
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <returns></returns>
	[HttpGet("getSubscriptionByMeasurementId/{measurementId}")]
	public async Task<ActionResult<SubscriptionDomain>> GetSubscriptionByMeasurementId (Guid measurementId)
	{
		logger.LogInformation("Запрос подписки для измерения с ID '{MeasurementId}'...", measurementId);

		SubscriptionDTO? subscription = await subscriptionsService.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);

		if (subscription == null)
		{
			logger.LogWarning("Подписка для измерения с ID '{MeasurementId}' не найдена", measurementId);
			return NotFound(new { message = $"Подписка с {nameof(measurementId)} = {measurementId} не найдена" });
		}
		else
		{
			logger.LogInformation("Подписка для измерения с ID '{MeasurementId}' найдена", measurementId);
			return Ok(subscription);
		}
	}

	/// <summary>
	/// Добавить подписку
	/// </summary>
	/// <param name="subscriptionDto"></param>
	/// <returns></returns>
	[HttpPost("addSubscription")]
	public async Task<IActionResult> AddSubscription ([FromBody] SubscriptionDTO subscriptionDto)
	{
		logger.LogInformation("Добавление подписки с топиком '{MqttTopic}'...", subscriptionDto?.MqttTopic);
		await subscriptionsService.AddSubscriptionAsync(subscriptionDto).ConfigureAwait(false);

		logger.LogInformation("Подписка '{MqttTopic}' добавлена", subscriptionDto.MqttTopic);
		return Ok(new { message = $"Подписка '{subscriptionDto.MqttTopic}' добавлена" });
	}

	/// <summary>
	/// Обновить подписку
	/// </summary>
	/// <param name="updatedSubscription"></param>
	/// <returns></returns>
	[HttpPut("updateSubscription")]
	public async Task<IActionResult> UpdateSubscription ([FromBody] SubscriptionDTO updatedSubscription)
	{
		logger.LogInformation("Обновление подписки с топиком '{MqttTopic}'...", updatedSubscription?.MqttTopic);
		await subscriptionsService.UpdateSubscriptionAsync(updatedSubscription).ConfigureAwait(false);

		logger.LogInformation("Подписка '{MqttTopic}' обновлена", updatedSubscription.MqttTopic);
		return Ok(new { message = $"Подписка '{updatedSubscription.MqttTopic}' обновлена" });
	}

	/// <summary>
	/// Удалить подписку
	/// </summary>
	/// <param name="measurementId"></param>
	/// <returns></returns>
	[HttpDelete("deleteSubscription/{measurementId}")]
	public async Task<IActionResult> DeleteSubscription (Guid measurementId)
	{
		logger.LogInformation("Удаление подписки с ID '{MeasurementId}'...", measurementId);
		await subscriptionsService.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);

		logger.LogInformation("Подписка с ID '{MeasurementId}' удалена", measurementId);
		return Ok(new { message = $"Подписка с ID '{measurementId}' удалена" });
	}
}
