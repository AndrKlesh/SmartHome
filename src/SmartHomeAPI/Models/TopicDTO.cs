#pragma warning disable CA1515

namespace SmartHomeAPI.Models;

public class TopicDTO
{
	public required string TopicName { get; set; }
	public required bool IsFavourite { get; set; }
}
