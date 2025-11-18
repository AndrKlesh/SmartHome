using Microsoft.AspNetCore.Mvc;
using SubscriptionsService.Abstractions.Models;
using SubscriptionsService.Abstractions.Services;

namespace SubscriptionsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController : ControllerBase
{
	private readonly ISubscriptionService _service;
	private readonly ILogger<SubscriptionsController> _logger;

	public SubscriptionsController (ISubscriptionService service, ILogger<SubscriptionsController> logger)
	{
		_service = service;
		_logger = logger;
	}

	[HttpGet("getAllSubscriptions")]
	public async Task<ActionResult<IReadOnlyList<SubscriptionDTO>>> GetAllSubscriptions ()
	{
		IReadOnlyList<SubscriptionDTO> subs = await _service.GetAllSubscriptionsAsync().ConfigureAwait(false);
		if (subs.Count == 0)
			return NotFound(new { message = "Список подписок пуст" });
		return Ok(subs);
	}

	[HttpGet("getSubscriptionByMeasurementId/{measurementId}")]
	public async Task<ActionResult<SubscriptionDTO>> GetSubscriptionByMeasurementId (Guid measurementId)
	{
		SubscriptionDTO sub = await _service.GetSubscriptionByMeasurementIdAsync(measurementId).ConfigureAwait(false);
		if (sub == null)
			return NotFound(new { message = $"Подписка {measurementId} не найдена" });
		return Ok(sub);
	}

	[HttpPost("addSubscription")]
	public async Task<IActionResult> AddSubscription ([FromBody] SubscriptionDTO dto)
	{
		await _service.AddSubscriptionAsync(dto).ConfigureAwait(false);
		return Ok(new { message = $"Подписка '{dto.MqttTopic}' добавлена" });
	}

	[HttpPut("updateSubscription")]
	public async Task<IActionResult> UpdateSubscription ([FromBody] SubscriptionDTO dto)
	{
		await _service.UpdateSubscriptionAsync(dto).ConfigureAwait(false);
		return Ok(new { message = $"Подписка '{dto.MqttTopic}' обновлена" });
	}

	[HttpDelete("deleteSubscription/{measurementId}")]
	public async Task<IActionResult> DeleteSubscription (Guid measurementId)
	{
		await _service.DeleteSubscriptionAsync(measurementId).ConfigureAwait(false);
		return Ok(new { message = $"Подписка {measurementId} удалена" });
	}
}
