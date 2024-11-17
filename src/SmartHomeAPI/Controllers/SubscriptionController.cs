using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController (SubscriptionsService subscriptionsService) : ControllerBase
{
	private readonly SubscriptionsService _subscriptionsService = subscriptionsService;

	[HttpGet("getAllSubscriptions")]
	public async Task<ActionResult<List<SubscriptionDomain>>> GetAllSubscriptions ()
	{
		List<SubscriptionDomain> subscriptions = await _subscriptionsService.GetAllSubscriptionsAsync();
		return Ok(subscriptions);
	}

	[HttpGet("getSubscriptionByMeasurementId/{measurementId}")]
	public async Task<ActionResult<SubscriptionDomain>> GetSubscriptionByMeasurementId (string measurementId)
	{
		SubscriptionDomain? subscription = await _subscriptionsService.GetSubscriptionByMeasurementIdAsync(measurementId);
		if (subscription == null)
		{
			return NotFound(new { message = $"Subscription with measurement ID '{measurementId}' not found." });
		}

		return Ok(subscription);
	}

	[HttpPost("addSubscription")]
	public async Task<IActionResult> AddSubscription ([FromBody] SubscriptionDTO subscriptionDto)
	{
		await _subscriptionsService.AddSubscriptionAsync(subscriptionDto);
		return Ok(new { message = "Subscription added successfully." });
	}

	[HttpPut("updateSubscription/{measurementId}")]
	public async Task<IActionResult> UpdateSubscription (string measurementId, [FromBody] SubscriptionDTO updatedSubscription)
	{
		try
		{
			await _subscriptionsService.UpdateSubscriptionAsync(measurementId, updatedSubscription);
			return Ok(new { message = "Subscription updated successfully." });
		}
		catch (ArgumentException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}

	[HttpDelete("deleteSubscription/{measurementId}")]
	public async Task<IActionResult> DeleteSubscription (string measurementId)
	{
		try
		{
			await _subscriptionsService.DeleteSubscriptionAsync(measurementId);
			return Ok(new { message = "Subscription deleted successfully." });
		}
		catch (ArgumentException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}
}
