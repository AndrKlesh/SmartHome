#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Интерфейс репозитория измерений
/// </summary>
public interface IMeasuresRepository
{
	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	/// <param name="measurement">Измерение</param>
	public Task AddMeasurementAsync (MeasureDomain measurement);

	/// <summary>
	/// Получить измерения по идентификатору типа измерения
	/// </summary>
	/// <param name="measurementId">идентификатор типа измерения</param>
	/// <returns>Список измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId);

	/// <summary>
	/// Получить последние измерения по идентификаторам их типов
	/// </summary>
	/// <param name="ids">идентификаторы типов измерений</param>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids);

	/// <summary>
	/// Получить последние значения всех измерений
	/// </summary>
	/// <returns>Список последних значений измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ();

	/// <summary>
	/// Получить историю измерений
	/// </summary>
	/// <param name="measurementId">идентификатор типа измерения</param>
	/// <param name="startDate">начальная дата</param>
	/// <param name="endDate">конечная дата</param>
	/// <returns>Список с историей измерений</returns>
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate);
}
