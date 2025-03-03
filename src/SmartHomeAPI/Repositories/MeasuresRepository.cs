#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий измерений
/// </summary>
public sealed class MeasuresRepository
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
		lock (_guard)
		{
			_measurements.Add(measurement);

			List<MeasureDomain> measurementsForTopic = _measurements.Where(m => m.MeasurementId == measurement.MeasurementId).ToList();
			if (measurementsForTopic.Count > MaxMeasurementsPerTopic)
			{
				MeasureDomain? oldestMeasurement = measurementsForTopic.OrderBy(m => m.Timestamp).FirstOrDefault();
				if (oldestMeasurement != null)
				{
					_ = _measurements.Remove(oldestMeasurement);
				}
			}
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Получить измерения по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">ид. типа измерения</param>
	/// <returns>Список измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId)
	{
		lock (_guard)
		{
			return Task.FromResult((IReadOnlyList<MeasureDomain>) _measurements.Where(m => m.MeasurementId == measurementId).ToArray());
		}
	}

	/// <summary>
	/// Получить полседние измерения по ид. их типов
	/// </summary>
	/// <param name="ids">ид. типов измерений</param>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids)
	{
		lock (_guard)
		{
			return Task.FromResult
			(
				(IReadOnlyList<MeasureDomain>) _measurements
														.Where(m => ids.Contains(m.MeasurementId))
														.GroupBy(m => m.MeasurementId)
														.Select(g => g.OrderByDescending(m => m.Timestamp).First())
														.ToArray()
			);
		}
	}

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		lock (_guard)
		{
			return Task.FromResult
			(
			  (IReadOnlyList<MeasureDomain>) _measurements
														.GroupBy(m => m.MeasurementId)
														.Select(g => g.OrderByDescending(m => m.Timestamp).First())
														.ToArray()
			);
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
		lock (_guard)
		{
			return Task.FromResult
			(
				(IReadOnlyList<MeasureDomain>) _measurements
														.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
														.OrderBy(m => m.Timestamp)
														.ToArray()
			);
		}
	}
}
