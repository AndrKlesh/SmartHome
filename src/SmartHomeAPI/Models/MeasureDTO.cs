#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

/// <summary>
/// DTO измерения
/// </summary>
public sealed class MeasureDTO
{
	/// <summary>
	/// Ид. типа измерения
	/// </summary>
	public Guid MeasurementId { get; set; }

	/// <summary>
	/// Имя измернеия
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Значение
	/// </summary>
	public string Value { get; set; } = string.Empty;

	/// <summary>
	/// Единицы измерения
	/// </summary>
	public string Units { get; set; } = string.Empty;

	/// <summary>
	/// Метка времени
	/// </summary>
	public DateTime Timestamp { get; set; }
}
