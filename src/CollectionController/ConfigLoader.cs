using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CollectionController.Models;

namespace CollectionController;
public static class ConfigLoader
{
	public static async Task<List<SchedulerConfig>?> LoadConfigAsync (string filePath)
	{
		// Читаем содержимое файла
		string json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);

		// Десериализуем JSON в список SchedulerConfig
		return JsonSerializer.Deserialize<List<SchedulerConfig>>(json);
	}
}
