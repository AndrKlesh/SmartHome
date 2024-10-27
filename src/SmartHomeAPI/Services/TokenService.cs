namespace SmartHomeAPI.Services;
public class TokenService (HttpClient httpClient)
{
	private readonly HttpClient _httpClient = httpClient;

	public async Task<string?> AuthenticateAsync (string username, string password)
	{
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5288/api/auth/login", new { username, password });
		if (response.IsSuccessStatusCode)
		{
			string token = await response.Content.ReadAsStringAsync();
			return token;
		}

		return null;
	}
}
