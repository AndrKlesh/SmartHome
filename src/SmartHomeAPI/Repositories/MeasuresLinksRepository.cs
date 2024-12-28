using System.Text.RegularExpressions;

namespace SmartHomeAPI.Repositories;

public class MeasuresLinksRepository
{
	public Task AddMeasureLinkAsync (string linkName, Guid MeasurementId)
	{
		throw new NotImplementedException();
	}

	public Task<Guid> GetMeasureIdAsync (string linkName)
	{
		return Task.FromResult(_storage [linkName]);
	}

	public Task<IReadOnlyList<KeyValuePair<string, Guid>>> FindLinksByMaskAsync (string mask) 
	{
		return Task.FromResult((IReadOnlyList<KeyValuePair<string, Guid>>) _storage.Where(item => Regex.IsMatch(item.Key, mask)).ToArray());
	}

	public Task<IReadOnlyList<Guid>> FindMeasuresIdByMaskAsync (string mask) 
	{
		return Task.FromResult((IReadOnlyList<Guid>)_storage.Where(item => Regex.IsMatch(item.Key, mask)).Select(item => item.Value).ToArray());
	}

	public Task RemoveMeasureLinkAsync (string linkName, Guid MeasurementId)
	{
		throw new NotImplementedException();
	}

	public Task RemoveMeasureLinkAsync (Guid MeasurementId)
	{
		throw new NotImplementedException();
	}

	private readonly Dictionary<string, Guid> _storage = new()
	{
		{ "home/h1/bathroom/hot_water_temp", Guid.Parse("462F9446-ADFF-4EA4-8CA1-F1665268520F") },
		{ "home/h1/living_room/temperature", Guid.Parse ("21274707-C7CA-4436-B191-9BAC91C473F5") },
		{ "home/h1/outside_temperature", Guid.Parse("24FE134B-4CBF-4EB9-A811-2720D4315146") },
		{ "home/h2/outside_temperature", Guid.Parse("24FE134B-4CBF-4EB9-A811-2720D4315146") }
	};
}
