using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

public class MeasuresStorageService
{
	private readonly List<TopicDomain> _topics = new();
	private readonly List<MeasureDomain> _measurements = new();
	private const int MaxMeasurementsPerTopic = 100;

	public async Task AddMeasureAsync (MeasureDTO measurementDto, string measurementId)
	{
		// Проверяем, существует ли топик в оперативной памяти
		TopicDomain? topic = _topics.FirstOrDefault(t => t.Id.ToString() == measurementId);

		if (topic == null)
		{
			// Если топик не найден, создаем новый
			topic = new TopicDomain { Id = Guid.Parse(measurementId), Name = measurementDto.TopicName };
			_topics.Add(topic);
			Console.WriteLine($"topic added: {topic.Name}");
		}

		// Создаем новое измерение на основе DTO
		MeasureDomain measurement = new()
		{
			TopicId = topic.Id,
			Value = measurementDto.Value,
			Timestamp = measurementDto.Timestamp,
			Topic = topic
		};

		// Добавляем новое измерение в память
		_measurements.Add(measurement);

		// Выводим содержимое _topics
		Console.WriteLine("Topics:");
		if (_topics.Count != 0)
		{
			Console.WriteLine(string.Join(", ", _topics.Select(t => t.Name)));
		}
		else
		{
			Console.WriteLine("No topics found");
		}

		// Выводим содержимое _measurements
		Console.WriteLine("Measurements:");
		if (_measurements.Count != 0)
		{
			Console.WriteLine(string.Join(", ", _measurements.Select(m => $"[Value: {m.Value}, Timestamp: {m.Timestamp}]")));
		}
		else
		{
			Console.WriteLine("No measurements found");
		}

		// Ограничиваем количество измерений до 100 для каждого топика
		List<MeasureDomain> measurementsForTopic = _measurements.Where(m => m.TopicId == topic.Id).ToList();
		if (measurementsForTopic.Count > MaxMeasurementsPerTopic)
		{
			// Находим самое старое измерение и удаляем его
			MeasureDomain? oldestMeasurement = measurementsForTopic.OrderBy(m => m.Timestamp).FirstOrDefault();
			if (oldestMeasurement != null)
			{
				_ = _measurements.Remove(oldestMeasurement);
			}
		}

		await Task.CompletedTask;
	}

	public async Task<List<MeasureWithFavouriteFlagDTO>> GetLatestMeasurementsAsync ()
	{
		// Получаем последние измерения для каждого топика
		List<MeasureDomain> latestMeasurements = _measurements
			.GroupBy(m => m.TopicId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToList();

		// Конвертируем в DTO
		return await Task.FromResult(latestMeasurements.Select(m => new MeasureWithFavouriteFlagDTO
		{
			TopicName = m.Topic?.Name ?? string.Empty,
			Value = m.Value,
			Timestamp = m.Timestamp,
			IsFavourite = m.Topic?.IsFavourite ?? false
		}).ToList());
	}

	public async Task ToggleFavouriteAsync (string measurementId, bool isFavourite)
	{
		// Находим топик по идентификатору
		TopicDomain? topic = _topics.FirstOrDefault(t => t.Id.ToString() == measurementId);

		if (topic == null)
		{
			throw new ArgumentNullException($"Topic with ID '{measurementId}' not found.");
		}

		// Обновляем значение поля IsFavourite
		topic.IsFavourite = isFavourite;
		await Task.CompletedTask;
	}

	public async Task<List<MeasuresHistoryDTO>> GetMeasurementsByTopicAndDateRangeAsync (string measurementId, DateTime startDate, DateTime endDate)
	{
		// Получаем измерения для указанного идентификатора и диапазона дат
		List<MeasureDomain> measurements = _measurements
			.Where(m => m.Topic?.Id.ToString() == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
			.OrderBy(m => m.Timestamp)
			.ToList();

		// Конвертируем в DTO
		return await Task.FromResult(measurements.Select(m => new MeasuresHistoryDTO
		{
			Value = m.Value,
			Timestamp = m.Timestamp
		}).ToList());
	}
}

/*using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

public class MeasuresStorageService
{
	public async Task AddMeasureAsync (MeasureDTO measurementDto)
	{
		using SmartHomeDbContext context = new();
		// Проверяем, существует ли топик в базе
		TopicDomain? topic = await context.Topics.FirstOrDefaultAsync(t => t.Name == measurementDto.TopicName);

		if (topic == null)
		{
			// Если топик не найден, создаем новый
			topic = new TopicDomain { Name = measurementDto.TopicName };
			_ = context.Topics.Add(topic);
			_ = await context.SaveChangesAsync(); // Сохраняем новый топик
		}

		// Создаем новое измерение на основе DTO
		MeasureDomain measurement = new()
		{
			TopicId = topic.Id,
			Value = measurementDto.Value,
			Timestamp = measurementDto.Timestamp,
			Topic = topic
		};

		// Добавляем и сохраняем новое измерение
		_ = context.Measurements.Add(measurement);
		_ = await context.SaveChangesAsync();
	}

	public async Task<List<MeasureWithFavouriteFlagDTO>> GetLatestMeasurementsAsync ()
	{
		using SmartHomeDbContext context = new();

		// Получаем последние измерения для каждого топика с данными о топиках
		List<MeasureDomain> latestMeasurements = await context.Measurements
			.Include(m => m.Topic) // Включаем данные о топике
			.GroupBy(m => m.TopicId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToListAsync();

		// Конвертируем в DTO
		return latestMeasurements.Select(m => new MeasureWithFavouriteFlagDTO
		{
			TopicName = m.Topic.Name, // Используем оператор безопасного обращения для предотвращения исключений
			Value = m.Value,
			Timestamp = m.Timestamp,
			IsFavourite = m.Topic.IsFavourite
		}).ToList();
	}

	public async Task ToggleFavouriteAsync (string topicName, bool isFavourite)
	{
		using SmartHomeDbContext context = new();

		// Находим топик по имени
		TopicDomain? topic = await context.Topics.FirstOrDefaultAsync(t => t.Name == topicName);

#pragma warning disable IDE0270
		if (topic == null)
		{
			throw new ArgumentNullException($"Topic with name '{topicName}' not found.");
		}
#pragma warning restore IDE0270

		// Обновляем значение поля IsFavourite
		topic.IsFavourite = isFavourite;

		// Сохраняем изменения в базе данных
		_ = await context.SaveChangesAsync();
	}

	public async Task<List<MeasuresHistoryDTO>> GetMeasurementsByTopicAndDateRangeAsync (string topicName, DateTime startDate, DateTime endDate)
	{
		using SmartHomeDbContext context = new();

		// Получаем измерения для указанного топика и диапазона дат
		List<MeasureDomain> measurements = await context.Measurements
			.Include(m => m.Topic)
			.Where(m => m.Topic.Name == topicName && m.Timestamp >= startDate && m.Timestamp <= endDate)
			.OrderBy(m => m.Timestamp)
			.ToListAsync();

		// Конвертируем в DTO
		return measurements.Select(m => new MeasuresHistoryDTO
		{
			Value = m.Value,
			Timestamp = m.Timestamp
		}).ToList();
	}
}
*/
