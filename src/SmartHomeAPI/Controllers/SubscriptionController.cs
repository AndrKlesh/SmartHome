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
public sealed class SubscriptionsController (SubscriptionService subscriptionsService) : Controller
{
	/// <summary>
	/// Получить все подписки
	/// </summary>
	/// <returns></returns>
	[HttpGet("getAllSubscriptions")]
	public async Task<ActionResult<IReadOnlyList<SubscriptionDomain>>> GetAllSubscriptions ()
	{
		IReadOnlyList<SubscriptionDTO> subscriptions = await subscriptionsService.GetAllSubscriptionsAsync().ConfigureAwait(false);
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
		SubscriptionDTO? subscription = await subscriptionsService.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		if (subscription == null)
		{
			return NotFound(new { message = $"Subscription with measurement ID '{measurementId}' not found." });
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
		await subscriptionsService.AddSubscriptionAsync(subscriptionDto).ConfigureAwait(false);
		return Ok(new { message = $"Subscription {subscriptionDto?.MqttTopic} added" });
	}

	/// <summary>
	/// Обновить подписку
	/// </summary>
	/// <param name="updatedSubscription"></param>
	/// <returns></returns>
	[HttpPut("updateSubscription")]
	public async Task<IActionResult> UpdateSubscription ([FromBody] SubscriptionDTO updatedSubscription)
	{
		try
		{
			await subscriptionsService.UpdateSubscriptionAsync(updatedSubscription).ConfigureAwait(false);
			return Ok(new { message = "Subscription updated successfully." });
		}
		catch (ArgumentException ex)
		{
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
		try
		{
			await subscriptionsService.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
			return Ok(new { message = "Subscription deleted successfully." });
		}
		catch (ArgumentException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}
}
