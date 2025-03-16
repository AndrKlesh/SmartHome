#pragma warning disable CA1515

using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Data;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Репозиторий измерений
/// </summary>
public sealed class MeasuresRepository (AppDbContext dbContext)
{
	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	/// <param name="measurement">Измерение</param>
	/// <returns></returns>
	public async Task AddMeasurementAsync (MeasureDomain measurement)
	{
		_ = await dbContext.Measurements.AddAsync(measurement).ConfigureAwait(false);
		_ = await dbContext.SaveChangesAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Получить измерения по ид. типа измерения
	/// </summary>
	/// <param name="measurementId">ид. типа измерения</param>
	/// <returns>Список измерений</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId)
	{
		return await dbContext.Measurements
			.Where(m => m.MeasurementId == measurementId)
			.ToArrayAsync()
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Получить полседние измерения по ид. их типов
	/// </summary>
	/// <param name="ids">ид. типов измерений</param>
	/// <returns>Список последних значений измерений</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids)
	{
		return await dbContext.Measurements
			.Where(m => ids.Contains(m.MeasurementId))
			.GroupBy(m => m.MeasurementId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToArrayAsync()
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	/// <returns>Список последних значений измерений</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		return await dbContext.Measurements
			.GroupBy(m => m.MeasurementId)
			.Select(g => g.OrderByDescending(m => m.Timestamp).First())
			.ToArrayAsync()
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Получить историю измерения
	/// </summary>
	/// <param name="measurementId">Ид. типа измерения</param>
	/// <param name="startDate">Дата начала</param>
	/// <param name="endDate">Дата конца</param>
	/// <returns>Список с историей измерения</returns>
	public async Task<IReadOnlyList<MeasureDomain>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		return await dbContext.Measurements
			.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
			.OrderBy(m => m.Timestamp)
			.ToArrayAsync()
			.ConfigureAwait(false);
	}
}
