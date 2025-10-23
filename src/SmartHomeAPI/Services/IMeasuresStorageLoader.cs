#pragma warning disable CA1515

using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

/// <summary>
/// Интерфейс чтения для сервиса хранения измерений
/// </summary>
public interface IMeasuresStorageLoader
{
	public Task<IReadOnlyList<MeasureDTO>> GetLatestMeasurementsAsync (string mask);
	public Task<IReadOnlyList<MeasureDTO>> SubscribeToLatestMeasurementsAsync (string mask);
	public Task<IReadOnlyList<MeasuresHistoryDTO>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate);
}
