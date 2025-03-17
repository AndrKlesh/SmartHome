#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

/// <summary>
/// DTO элемента истории измерения
/// </summary>
public sealed class MeasuresHistoryDTO
{
	/// <summary>
	/// Значение измерения
	/// </summary>
	public string Value { get; set; } = string.Empty;

	/// <summary>
	/// Метка времени
	/// </summary>
	public DateTime Timestamp { get; set; }
}
