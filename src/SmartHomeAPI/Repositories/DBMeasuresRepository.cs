#pragma warning disable CA1515

using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Data;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий измерений
/// </summary>
public sealed class DBMeasuresRepository (AppDbContext dbContext, ILogger<DBMeasuresRepository> logger) : IMeasuresRepository
{
	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	/// <param name="measurement">Измерение</param>
	/// <returns></returns>
	public async Task AddMeasurementAsync (MeasureDomain measurement)
	{
		if (measurement == null)
		{
			logger.LogWarning("Попытка добавить null-измерение");
			return;
		}

		logger.LogInformation("Добавление измерения с ID '{MeasurementId}'...", measurement.MeasurementId);

		_ = await dbContext.Measurements.AddAsync(measurement).ConfigureAwait(false);
		_ = await dbContext.SaveChangesAsync().ConfigureAwait(false);

		logger.LogInformation("Добавление измерения с ID '{MeasurementId}' завершено", measurement.MeasurementId);
	}

	/// <summary>
	/// Получить измерения по ид. типа измерения
	/// </summary>
	public async Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId)
	{
		logger.LogInformation("Получение измерений для ID типа измерения '{MeasurementId}'...", measurementId);

		MeasureDomain [] measurements = await dbContext.Measurements
			.Where(m => m.MeasurementId == measurementId)
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (measurements.Length == 0)
		{
			logger.LogInformation("Измерений для ID типа измерения '{MeasurementId}' не найдено", measurementId);
		}
		else
		{
			logger.LogInformation("Для типа измерения '{MeasurementId}' найдено {Count} записей", measurementId, measurements.Length);
		}

		return measurements;
	}

	/// <summary>
	/// Получить последние измерения по ид. их типов
	/// </summary>
	public async Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids)
	{
		if (ids == null || ids.Count == 0)
		{
			logger.LogWarning("Попытка получить последние измерения с пустым списком ID");
			return Array.Empty<MeasureDomain>();
		}

		logger.LogInformation("Получение последних измерений для типов: {MeasurementIds}...", string.Join(", ", ids));

		MeasureDomain [] measurements = await dbContext.Measurements
			.Where(m => ids.Contains(m.MeasurementId))
			.GroupBy(m => m.MeasurementId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (measurements.Length == 0)
		{
			logger.LogInformation("Последние измерения для типов: {MeasurementIds} не найдены", string.Join(", ", ids));
		}
		else
		{
			logger.LogInformation("Найдено {Count} последних измерений для заданных типов", measurements.Length);
		}

		return measurements;
	}

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	public async Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		logger.LogInformation("Получение последних измерений для всех типов...");

		MeasureDomain [] measurements = await dbContext.Measurements
			.GroupBy(m => m.MeasurementId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (measurements.Length == 0)
		{
			logger.LogInformation("Последние измерения для всех типов не найдены");
		}
		else
		{
			logger.LogInformation("Найдено {Count} последних измерений для всех типов", measurements.Length);
		}

		return measurements;
	}

	/// <summary>
	/// Получить историю измерения
	/// </summary>
	public async Task<IReadOnlyList<MeasureDomain>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		if (startDate > endDate)
		{
			logger.LogWarning("Некорректный интервал: startDate ({StartDate}) > endDate ({EndDate})", startDate, endDate);
			return Array.Empty<MeasureDomain>();
		}

		logger.LogInformation("Получение истории измерений для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}'...", measurementId, startDate, endDate);

		MeasureDomain [] measurements = await dbContext.Measurements
			.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
			.OrderBy(m => m.Timestamp)
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (measurements.Length == 0)
		{
			logger.LogInformation("Измерения для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}' не найдены", measurementId, startDate, endDate);
		}
		else
		{
			logger.LogInformation("Для типа '{MeasurementId}' найдено {Count} измерений с '{StartDate}' по '{EndDate}'", measurementId, measurements.Length, startDate, endDate);
		}

		return measurements;
	}
}
