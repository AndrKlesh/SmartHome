#pragma warning disable CA1515

namespace AuthService.Models;

public sealed class User
{
	public required string Username { get; set; }
	public required string Password { get; set; }
}
