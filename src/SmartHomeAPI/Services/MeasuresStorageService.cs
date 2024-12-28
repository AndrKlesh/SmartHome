#pragma warning disable CA1515

using System.Diagnostics.CodeAnalysis;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

public class MeasuresStorageService (MeasurementRepository measurementRepository, 
	                                 FavouritesMeasuresRepository favouritesRepository,
									 SubscriptionRepository subscriptionRepository,
									 MeasuresLinksRepository measuresLinksRepository)
{
	private readonly MeasurementRepository _measurementRepository = measurementRepository;
	private readonly FavouritesMeasuresRepository _favouritesRepository = favouritesRepository;
	private readonly SubscriptionRepository _subscriptionRepository = subscriptionRepository;
	private readonly MeasuresLinksRepository _measuresLinksRepository = measuresLinksRepository;

	public async Task AddMeasureAsync ([NotNull]MeasureDTO measurementDto)
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
		IReadOnlyList<KeyValuePair<string, Guid>> measurementsLinks = await _measuresLinksRepository.FindLinksByMaskAsync("home/h1/*") //TODO Права доступа пользователей
			                                                                                        .ConfigureAwait(false);
		Task<List<MeasureDomain>> latestMeasuresTask = _measurementRepository.GetLatestMeasurementsAsync(measurementsLinks.Select(l => l.Value).ToArray());
		Task<IReadOnlyList<FavoritesDomain>> favoritesTask = _favouritesRepository.GetFavoritesMeasuresIdsAsync();

		await Task.WhenAll(latestMeasuresTask, favoritesTask).ConfigureAwait(false);
		List<MeasureDomain> latestMeasuresDomain = await latestMeasuresTask.ConfigureAwait(false);
		IReadOnlyList<FavoritesDomain> favoritesDomain = await favoritesTask.ConfigureAwait(false);

		List<MeasureDTO> latestMeasurementsDTO = new(latestMeasuresDomain.Count);
		foreach (MeasureDomain measure in latestMeasuresDomain)
		{
			SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMeasurementIdAsync(measure.MeasurementId)
				                                                            .ConfigureAwait(false);
			if (subscription is null) 
			{
				continue;
			}

			latestMeasurementsDTO.Add(new MeasureDTO()
			{
				MeasurementId = measure.MeasurementId,
				MeasurementTag = measurementsLinks.FirstOrDefault(l => l.Value == measure.MeasurementId).Key,
				Name = subscription.MeasurementName,
				Units = subscription.Unit,
				Timestamp = measure.Timestamp,
				Value = measure.Value,
				IsFavourite = favoritesDomain.Any(f => f.MeasureId == measure.MeasurementId)
			});
		}
		return latestMeasurementsDTO;
	}

	public async Task<List<MeasuresHistoryDTO>> GetMeasurementsByTopicAndDateRangeAsync (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		List<MeasureDomain> measurements = await _measurementRepository.GetMeasurementsByTopicAndDateRangeAsync(measurementId, startDate, endDate).ConfigureAwait(false);

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
