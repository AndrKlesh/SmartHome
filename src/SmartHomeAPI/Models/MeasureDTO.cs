#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

public class MeasureDTO
{
	public string MeasurementId { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
	public string Units { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
	public bool IsFavourite { get; set; }
}
