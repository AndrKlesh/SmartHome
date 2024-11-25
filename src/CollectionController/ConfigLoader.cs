using System.Text.Json;
using CollectionController.Models;

namespace CollectionController;
public static class ConfigLoader
{
	public static async Task<List<SchedulerConfig>?> LoadConfigAsync (string filePath)
	{
		using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);

		return await JsonSerializer.DeserializeAsync<List<SchedulerConfig>>(stream).ConfigureAwait(false);
	}
}
