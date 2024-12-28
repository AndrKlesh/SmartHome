#pragma warning disable CA1062
#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController (SubscriptionService subscriptionsService) : ControllerBase
{
	private readonly SubscriptionService _subscriptionsService = subscriptionsService;

	[HttpGet("getAllSubscriptions")]
	public async Task<ActionResult<List<SubscriptionDomain>>> GetAllSubscriptions ()
	{
		List<SubscriptionDTO> subscriptions = await _subscriptionsService.GetAllSubscriptionsAsync().ConfigureAwait(false);
		return Ok(subscriptions);
	}

	[HttpGet("getSubscriptionByMeasurementId/{measurementId}")]
	public async Task<ActionResult<SubscriptionDomain>> GetSubscriptionByMeasurementId (Guid measurementId)
	{
		SubscriptionDTO? subscription = await _subscriptionsService.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		if (subscription == null)
		{
			return NotFound(new { message = $"Subscription with measurement ID '{measurementId}' not found." });
		}

		return Ok(subscription);
	}

	[HttpPost("addSubscription")]
	public async Task<IActionResult> AddSubscription ([FromBody] SubscriptionDTO subscriptionDto)
	{
		await _subscriptionsService.AddSubscriptionAsync(subscriptionDto).ConfigureAwait(false);
		return Ok(new { message = $"Subscription {subscriptionDto.MqttTopic} added" });
	}

	[HttpPut("updateSubscription/{measurementId}")]
	public async Task<IActionResult> UpdateSubscription (Guid measurementId, [FromBody] SubscriptionDTO updatedSubscription)
	{
		try
		{
			await _subscriptionsService.UpdateSubscriptionAsync(measurementId, updatedSubscription).ConfigureAwait(false);
			return Ok(new { message = "Subscription updated successfully." });
		}
		catch (ArgumentException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}

	[HttpDelete("deleteSubscription/{measurementId}")]
	public async Task<IActionResult> DeleteSubscription (Guid measurementId)
	{
		try
		{
			await _subscriptionsService.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
			return Ok(new { message = "Subscription deleted successfully." });
		}
		catch (ArgumentException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}
}
