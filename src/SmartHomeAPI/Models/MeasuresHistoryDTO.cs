#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

public class MeasuresHistoryDTO
{
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
}
