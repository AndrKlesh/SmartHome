#pragma warning disable CA1515

using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Data;
using SmartHomeAPI.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий измерений
/// </summary>
public sealed class MeasuresRepository(AppDbContext dbContext, ILogger<MeasuresRepository> logger)
{
	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	/// <param name="measurement">Измерение</param>
	/// <returns></returns>
	public async Task AddMeasurementAsync(MeasureDomain measurement)
	{
		if (measurement == null)
		{
			logger.LogWarning("Попытка добавить null-измерение");
			return;
		}

		logger.LogInformation("Добавление измерения с ID '{MeasurementId}'...", measurement.MeasurementId);

		await dbContext.Measurements.AddAsync(measurement).ConfigureAwait(false);
		await dbContext.SaveChangesAsync().ConfigureAwait(false);

		logger.LogInformation("Добавление измерения с ID '{MeasurementId}' завершено", measurement.MeasurementId);
	}

	/// <summary>
	/// Получить измерения по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">ид. типа измерения</param>
	/// <returns>Список измерений</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync(Guid measurementId)
	{
		logger.LogInformation("Получение измерений для ID типа измерения '{MeasurementId}'...", measurementId);

		var measurements = await dbContext.Measurements
			.Where(m => m.MeasurementId == measurementId)
			.ToArrayAsync()
			.ConfigureAwait(false);

		logger.LogInformation(measurements.Length == 0
			? "Измерений для ID типа измерения '{MeasurementId}' не найдено"
			: "Для типа измерения '{MeasurementId}' найдено {Count} записей", measurementId, measurements.Length);

		return measurements;
	}

	/// <summary>
	/// Получить последние измерения по ид. их типов
	/// </summary>
	/// <param name="ids">ид. типов измерений</param>
	/// <returns>Список последних значений измерений</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync(IReadOnlyList<Guid> ids)
	{
		if (ids == null || ids.Count == 0)
		{
			logger.LogWarning("Попытка получить последние измерения с пустым списком ID");
			return Array.Empty<MeasureDomain>();
		}

		logger.LogInformation("Получение последних измерений для типов: {MeasurementIds}...", string.Join(", ", ids));

		var measurements = await dbContext.Measurements
			.Where(m => ids.Contains(m.MeasurementId))
			.GroupBy(m => m.MeasurementId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToArrayAsync()
			.ConfigureAwait(false);

		logger.LogInformation(measurements.Length == 0
			? "Последние измерения для типов: {MeasurementIds} не найдены"
			: "Найдено {Count} последних измерений для заданных типов", string.Join(", ", ids), measurements.Length);

		return measurements;
	}

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	/// <returns>Список последних значений измерений</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync()
	{
		logger.LogInformation("Получение последних измерений для всех типов...");

		var measurements = await dbContext.Measurements
			.GroupBy(m => m.MeasurementId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToArrayAsync()
			.ConfigureAwait(false);

		logger.LogInformation(measurements.Length == 0
			? "Последние измерения для всех типов не найдены"
			: "Найдено {Count} последних измерений для всех типов", measurements.Length);

		return measurements;
	}

	/// <summary>
	/// Получить историю измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <param name="startDate">Дата начала</param>
	/// <param name="endDate">Дата конца</param>
	/// <returns>Список с историей измерения</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetMeasurementHistory(Guid measurementId, DateTime startDate, DateTime endDate)
	{
		if (startDate > endDate)
		{
			logger.LogWarning("Некорректный интервал: startDate ({StartDate}) > endDate ({EndDate})", startDate, endDate);
			return Array.Empty<MeasureDomain>();
		}

		logger.LogInformation("Получение истории измерений для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}'...", measurementId, startDate, endDate);

		var measurements = await dbContext.Measurements
			.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
			.OrderBy(m => m.Timestamp)
			.ToArrayAsync()
			.ConfigureAwait(false);

		logger.LogInformation(measurements.Length == 0
			? "Измерения для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}' не найдены"
			: "Для типа '{MeasurementId}' найдено {Count} измерений с '{StartDate}' по '{EndDate}'", measurementId, measurements.Length, startDate, endDate);

		return measurements;
	}
}
