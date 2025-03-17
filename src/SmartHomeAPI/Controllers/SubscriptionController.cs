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
		logger.LogInformation("Запрос всех подписок");
		IReadOnlyList<SubscriptionDTO> subscriptions = await subscriptionsService.GetAllSubscriptionsAsync().ConfigureAwait(false);

		if (subscriptions.Count == 0)
		{
			logger.LogWarning("Список подписок пуст");
		}

		return Ok(subscriptions);
	}

	/// <summary>
	/// Получить подписку
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <returns></returns>
	[HttpGet("getSubscriptionByMeasurementId/{measurementId}")]
	public async Task<ActionResult<SubscriptionDomain>> GetSubscriptionByMeasurementId (Guid measurementId)
	{
		logger.LogInformation("Запрос подписки для измерения с ID {MeasurementId}", measurementId);

		SubscriptionDTO? subscription = await subscriptionsService.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		if (subscription == null)
		{
			logger.LogWarning("Подписка для измерения с ID {MeasurementId} не найдена", measurementId);
			return NotFound(new { message = $"Подписка с {nameof(measurementId)} = {measurementId} не найдена" });
		}

		return Ok(subscription);
	}

	/// <summary>
	/// Добавить подписку
	/// </summary>
	/// <param name="subscriptionDto"></param>
	/// <returns></returns>
	[HttpPost("addSubscription")]
	public async Task<IActionResult> AddSubscription ([FromBody] SubscriptionDTO subscriptionDto)
	{
		logger.LogInformation("Добавление подписки с топиком {MqttTopic}", subscriptionDto?.MqttTopic);
		await subscriptionsService.AddSubscriptionAsync(subscriptionDto).ConfigureAwait(false);

		logger.LogInformation("Подписка {MqttTopic} успешно добавлена", subscriptionDto.MqttTopic);
		return Ok(new { message = $"Подписка {subscriptionDto.MqttTopic} добавлена" });
	}

	/// <summary>
	/// Обновить подписку
	/// </summary>
	/// <param name="updatedSubscription"></param>
	/// <returns></returns>
	[HttpPut("updateSubscription")]
	public async Task<IActionResult> UpdateSubscription ([FromBody] SubscriptionDTO updatedSubscription)
	{
		logger.LogInformation("Обновление подписки с топиком {MqttTopic}", updatedSubscription?.MqttTopic);

		try
		{
			await subscriptionsService.UpdateSubscriptionAsync(updatedSubscription).ConfigureAwait(false);
			logger.LogInformation("Подписка {MqttTopic} успешно обновлена", updatedSubscription.MqttTopic);
			return Ok(new { message = $"Подписка {updatedSubscription.MqttTopic} обновлена" });
		}
		catch (Exception ex)
		{
			logger.LogWarning("Ошибка при обновлении подписки {MqttTopic}: {Message}", updatedSubscription.MqttTopic, ex.Message);
			return NotFound(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Удалить подписку
	/// </summary>
	/// <param name="measurementId"></param>
	/// <returns></returns>
	[HttpDelete("deleteSubscription/{measurementId}")]
	public async Task<IActionResult> DeleteSubscription (Guid measurementId)
	{
		logger.LogInformation("Удаление подписки с ID {MeasurementId}", measurementId);

		try
		{
			await subscriptionsService.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
			logger.LogInformation("Подписка с ID {MeasurementId} успешно удалена", measurementId);
			return Ok(new { message = $"Подписка {measurementId} удалена" });
		}
		catch (Exception ex)
		{
			logger.LogWarning("Ошибка при удалении подписки с ID {MeasurementId}: {Message}", measurementId, ex.Message);
			return NotFound(new { message = ex.Message });
		}
	}
}
