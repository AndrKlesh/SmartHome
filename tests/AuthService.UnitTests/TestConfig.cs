using Microsoft.Extensions.Configuration;

namespace AuthService.UnitTests;

internal static class TestConfig
{
	public static IConfiguration Build ()
	{
		Dictionary<string, string?> data = new()
		{
			["Jwt:Key"] = "F8h#9sLm@2vX!zP$QeRtY&5*KdNwUoG1",
			["users:0:login"] = "user",
			["users:0:passwordSha512"] = ComputeSha512Hex("user"),
			["users:0:role"] = "User",
		};
		return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
	}

	private static string ComputeSha512Hex (string input)
	{
		byte [] bytes = System.Text.Encoding.UTF8.GetBytes(input);
		byte [] hash = System.Security.Cryptography.SHA512.HashData(bytes);
		var sb = new System.Text.StringBuilder(hash.Length * 2);
		foreach (byte b in hash) sb.Append(b.ToString("x2"));
		return sb.ToString();
	}
}
