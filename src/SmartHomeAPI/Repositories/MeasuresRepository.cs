#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий измерений
/// </summary>
public sealed class MeasuresRepository (ILogger<MeasuresRepository> logger)
{
	private readonly List<MeasureDomain> _measurements = new();
	private readonly Lock _guard = new();
	private const int MaxMeasurementsPerTopic = 100;

	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	/// <param name="measurement">Измерение</param>
	/// <returns></returns>
	public Task AddMeasurementAsync (MeasureDomain measurement)
	{
		logger.LogInformation("Добавление измерения с ID '{MeasurementId}'...", measurement?.MeasurementId);

		lock (_guard)
		{
			_measurements.Add(measurement);

			List<MeasureDomain> measurementsForTopic = _measurements.Where(m => m.MeasurementId == measurement.MeasurementId).ToList();
			if (measurementsForTopic.Count > MaxMeasurementsPerTopic)
			{
				MeasureDomain? oldestMeasurement = measurementsForTopic.OrderBy(m => m.Timestamp).FirstOrDefault();
				if (oldestMeasurement != null)
				{
					logger.LogInformation("Удаление самого старого измерения с ID '{MeasurementId}'", oldestMeasurement.MeasurementId);
					_ = _measurements.Remove(oldestMeasurement);
				}
			}
		}

		logger.LogInformation("Измерение с ID '{MeasurementId}' добавлено", measurement.MeasurementId);
		return Task.CompletedTask;
	}

	/// <summary>
	/// Получить измерения по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">ид. типа измерения</param>
	/// <returns>Список измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId)
	{
		logger.LogInformation("Получение измерений для ID типа измерения '{MeasurementId}'...", measurementId);

		lock (_guard)
		{
			MeasureDomain [] measurements = _measurements.Where(m => m.MeasurementId == measurementId).ToArray();
			logger.LogInformation("Найдено {Count} измерений для типа '{MeasurementId}'", measurements.Length, measurementId);
			return Task.FromResult((IReadOnlyList<MeasureDomain>) measurements);
		}
	}

	/// <summary>
	/// Получить полседние измерения по ид. их типов
	/// </summary>
	/// <param name="ids">ид. типов измерений</param>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids)
	{
		logger.LogInformation("Получение последних измерений для типов: '{MeasurementIds}'...", string.Join(", ", ids));

		lock (_guard)
		{
			MeasureDomain [] measurements = _measurements
				.Where(m => ids.Contains(m.MeasurementId))
				.GroupBy(m => m.MeasurementId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToArray();

			logger.LogInformation("Найдено {Count} последних измерений для заданных типов", measurements.Length);
			return Task.FromResult((IReadOnlyList<MeasureDomain>) measurements);
		}
	}

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		logger.LogInformation("Получение последних измерений для всех типов...");

		lock (_guard)
		{
			MeasureDomain [] measurements = _measurements
				.GroupBy(m => m.MeasurementId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToArray();

			logger.LogInformation("Найдено {Count} последних измерений для всех типов", measurements.Length);
			return Task.FromResult((IReadOnlyList<MeasureDomain>) measurements);
		}
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
		logger.LogInformation("Получение истории измерений для типа '{MeasurementId}' с '{StartDate}' по '{EndDate}'...", measurementId, startDate, endDate);

		lock (_guard)
		{
			MeasureDomain [] history = _measurements
				.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
				.OrderBy(m => m.Timestamp)
				.ToArray();

			logger.LogInformation("Найдено {Count} измерений для типа '{MeasurementId}' в указанном интервале", history.Length, measurementId);
			return Task.FromResult((IReadOnlyList<MeasureDomain>) history);
		}
	}
}
