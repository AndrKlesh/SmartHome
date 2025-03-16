#pragma warning disable CA1515

using System.ComponentModel.DataAnnotations;

namespace SmartHomeAPI.Entities;

/// <summary>
/// Модель измерения
/// </summary>
public sealed class MeasureDomain
{
	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid MeasurementId { get; set; }
	/// <summary>
	/// Значение
	/// </summary>
	[Required]
	public string Value { get; set; } = string.Empty;
	/// <summary>
	/// Метка времени
	/// </summary>
	[Required]
	public DateTime Timestamp { get; set; }
}
