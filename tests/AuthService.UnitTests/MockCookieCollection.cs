using Microsoft.AspNetCore.Http;

namespace AuthService.UnitTests;

internal class MockCookieCollection (Dictionary<string, string> cookies) : IRequestCookieCollection
{
	private readonly Dictionary<string, string> _cookies = cookies;

	public string this [string key] => _cookies.TryGetValue(key, out var value) ? value : null;

	public int Count => _cookies.Count;

	public ICollection<string> Keys => _cookies.Keys;

	public bool ContainsKey (string key)
	{
		return _cookies.ContainsKey(key);
	}

	public IEnumerator<KeyValuePair<string, string>> GetEnumerator ()
	{
		return _cookies.GetEnumerator();
	}

	public bool TryGetValue (string key, out string value)
	{
		return _cookies.TryGetValue(key, out value);
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	{
		return GetEnumerator();
	}
}
