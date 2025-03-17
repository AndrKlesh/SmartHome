#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

/// <summary>
/// DTO подписки на mqtt-топики
/// </summary>
public sealed class SubscriptionDTO
{
	/// <summary>
	/// Ид. типа измерения
	/// </summary>
	public Guid MeasurementId { get; set; }

	/// <summary>
	/// Описание измерения
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Единицы измерения
	/// </summary>
	public string Unit { get; set; } = string.Empty;

	/// <summary>
	/// Mqtt-топик
	/// </summary>
	public string MqttTopic { get; set; } = string.Empty;
}
