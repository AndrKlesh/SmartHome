#pragma warning disable CA1515

namespace SmartHomeAPI.Entities;

public class User
{
	public required string Username { get; set; }
	public required string Password { get; set; }
}
