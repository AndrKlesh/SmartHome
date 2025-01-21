#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

/// <summary>
/// Модель измерения
/// </summary>
public class MeasureDomain
{
	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid MeasurementId { get; set; }
	/// <summary>
	/// Значение
	/// </summary>
	public string Value { get; set; } = string.Empty;
	/// <summary>
	/// Метка времени
	/// </summary>
	public DateTime Timestamp { get; set; }
}
