#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

/// <summary>
/// Модель подписки на mqtt-топик
/// </summary>
public class SubscriptionDomain
{
	/// <summary>
	/// Идентификатор типа измерения
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
	/// <summary>
	/// Имя конвертера
	/// </summary>
	public string ConverterName { get; set; } = "default";
}
