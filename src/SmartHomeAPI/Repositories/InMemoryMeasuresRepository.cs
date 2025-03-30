#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий измерений
/// </summary>
public sealed class InMemoryMeasuresRepository (ILogger<InMemoryMeasuresRepository> logger) : IMeasuresRepository
{
	private readonly List<MeasureDomain> _measurements = new();
	private readonly Lock _lock = new();
	private const int MaxMeasurementsPerTopic = 100;

	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	/// <param name="measurement">Измерение</param>
	/// <returns></returns>
	public Task AddMeasurementAsync (MeasureDomain measurement)
	{
		if (measurement == null)
		{
			logger.LogWarning("Попытка добавить null-измерение");
			return Task.CompletedTask;
		}

		logger.LogInformation("Добавление измерения с ID '{MeasurementId}'...", measurement.MeasurementId);

		MeasureDomain? oldestMeasurement = null;

		lock (_lock)
		{
			_measurements.Add(measurement);

			List<MeasureDomain> measurementsForTopic = _measurements
				.Where(m => m.MeasurementId == measurement.MeasurementId)
				.OrderBy(m => m.Timestamp)
				.ToList();

			if (measurementsForTopic.Count > MaxMeasurementsPerTopic)
			{
				oldestMeasurement = measurementsForTopic.First();
			}

			if (oldestMeasurement != null)
			{

				if (_measurements.Remove(oldestMeasurement))
				{
					logger.LogInformation("Удалено самое старое измерение с ID '{MeasurementId}'", oldestMeasurement.MeasurementId);
				}
				else
				{
					logger.LogWarning("Не удалось удалить самое старое измерение с ID '{MeasurementId}'", oldestMeasurement.MeasurementId);
				}
			}

			logger.LogInformation("Добавление измерения с ID '{MeasurementId}' завершено", measurement.MeasurementId);
			return Task.CompletedTask;
		}
	}

	/// <summary>
	/// Получить измерения по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">ид. типа измерения</param>
	/// <returns>Список измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId)
	{
		logger.LogInformation("Получение измерений для ID типа измерения '{MeasurementId}'...", measurementId);

		MeasureDomain [] measurements;

		lock (_lock)
		{
			measurements = _measurements
				.Where(m => m.MeasurementId == measurementId)
				.ToArray();
		}

		if (measurements.Length == 0)
		{
			logger.LogInformation("Измерений для ID типа измерения '{MeasurementId}' не найдено", measurementId);
		}
		else
		{
			logger.LogInformation("Для типа измерения '{MeasurementId}' найдено {Count} записей", measurementId, measurements.Length);
		}

		return Task.FromResult<IReadOnlyList<MeasureDomain>>(measurements);
	}

	/// <summary>
	/// Получить полседние измерения по ид. их типов
	/// </summary>
	/// <param name="ids">ид. типов измерений</param>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids)
	{
		if (ids == null || ids.Count == 0)
		{
			logger.LogWarning("Попытка получить последние измерения с пустым списком ID");
			return Task.FromResult<IReadOnlyList<MeasureDomain>>(Array.Empty<MeasureDomain>());
		}

		logger.LogInformation("Получение последних измерений для типов: {MeasurementIds}...", string.Join(", ", ids));

		MeasureDomain [] measurements;

		lock (_lock)
		{
			measurements = _measurements
				.Where(m => ids.Contains(m.MeasurementId))
				.GroupBy(m => m.MeasurementId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToArray();
		}

		if (measurements.Length == 0)
		{
			logger.LogWarning("Последние измерения для типов: {MeasurementIds} не найдены", string.Join(", ", ids));
		}
		else
		{
			logger.LogInformation("Найдено {Count} последних измерений для заданных типов", measurements.Length);
		}

		return Task.FromResult<IReadOnlyList<MeasureDomain>>(measurements);
	}

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		logger.LogInformation("Получение последних измерений для всех типов...");

		MeasureDomain [] measurements;

		lock (_lock)
		{
			measurements = _measurements
				.GroupBy(m => m.MeasurementId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToArray();
		}

		if (measurements.Length == 0)
		{
			logger.LogWarning("Последние измерения для всех типов не найдены");
		}
		else
		{
			logger.LogInformation("Найдено {Count} последних измерений для всех типов", measurements.Length);
		}

		return Task.FromResult<IReadOnlyList<MeasureDomain>>(measurements);
	}

	/// <summary>
	/// Получить историю измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <param name="startDate">Дата начала</param>
	/// <param name="endDate">Дата конца</param>
	/// <returns>Список с историей измерения</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		if (startDate > endDate)
		{
			logger.LogWarning("Некорректный интервал: startDate ({StartDate}) > endDate ({EndDate})", startDate, endDate);
			return Task.FromResult<IReadOnlyList<MeasureDomain>>(Array.Empty<MeasureDomain>());
		}

		logger.LogInformation("Получение истории измерений для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}'...", measurementId, startDate, endDate);

		MeasureDomain [] measurements;

		lock (_lock)
		{
			measurements = _measurements
				.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
				.OrderBy(m => m.Timestamp)
				.ToArray();
		}

		if (measurements.Length == 0)
		{
			logger.LogWarning("Измерения для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}' не найдены", measurementId, startDate, endDate);
		}
		else
		{
			logger.LogInformation("Для типа '{MeasurementId}' найдено {Count} измерений с '{StartDate}' по '{EndDate}'", measurementId, measurements.Length, startDate, endDate);
		}

		return Task.FromResult<IReadOnlyList<MeasureDomain>>(measurements);
	}
}
