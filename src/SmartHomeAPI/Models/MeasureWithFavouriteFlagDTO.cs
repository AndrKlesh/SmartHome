namespace SmartHomeAPI.Models;

public class MeasureWithFavouriteFlagDTO
{
	public string TopicName { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
	public bool IsFavourite { get; set; }
}
