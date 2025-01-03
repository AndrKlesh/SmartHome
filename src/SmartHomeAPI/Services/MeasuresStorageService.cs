#pragma warning disable CA1515

using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

public class MeasuresStorageService (MeasurementRepository measurementRepository)
{
	private readonly MeasurementRepository _measurementRepository = measurementRepository;

	public async Task AddMeasureAsync (MeasureDTO measurementDto)
	{
		MeasureDomain measurement = new()
		{
			MeasurementId = measurementDto.MeasurementId,
			Value = measurementDto.Value,
			Timestamp = measurementDto.Timestamp
		};

		await _measurementRepository.AddMeasurementAsync(measurement).ConfigureAwait(false);
	}

	public async Task<List<MeasureDTO>> GetLatestMeasurementsAsync ()
	{
		List<MeasureDomain> latestMeasurements = await _measurementRepository.GetLatestMeasurementsAsync().ConfigureAwait(false);

		return latestMeasurements.Select(m => new MeasureDTO
		{
			MeasurementId = m.MeasurementId ?? string.Empty,
			Value = m.Value,
			Timestamp = m.Timestamp
		}).ToList();
	}

/*	public async Task ToggleFavouriteAsync (string topicName, bool isFavourite)
	{
		await _topicRepository.ToggleFavouriteAsync(topicName, isFavourite).ConfigureAwait(false);
	}*/

	public async Task<List<MeasuresHistoryDTO>> GetMeasurementsByTopicAndDateRangeAsync (string topicName, DateTime startDate, DateTime endDate)
	{
		List<MeasureDomain> measurements = await _measurementRepository.GetMeasurementsByTopicAndDateRangeAsync(topicName, startDate, endDate).ConfigureAwait(false);

		return measurements.Select(m => new MeasuresHistoryDTO
		{
			Value = m.Value,
			Timestamp = m.Timestamp
		}).ToList();
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
