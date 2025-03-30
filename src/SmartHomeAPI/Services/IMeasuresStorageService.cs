#pragma warning disable CA1515

using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

/// <summary>
/// Интерфейс сервиса измерений
/// </summary>
public interface IMeasuresStorageService
{
	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	public Task AddMeasureAsync (MeasureDTO measurementDto);

	/// <summary>
	/// Подписаться на последние измерения
	/// </summary>
	public Task<IReadOnlyList<MeasureDTO>> SubscribeToLatestMeasurementsAsync (string mask);

	/// <summary>
	/// Получить последние измерения по маске.
	/// </summary>
	public Task<IReadOnlyList<MeasureDTO>> GetLatestMeasurementsAsync (string mask);

	/// <summary>
	/// Получить историю измерений
	/// </summary>
	public Task<IReadOnlyList<MeasuresHistoryDTO>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate);
}
