#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Интерфейс чтения для репозитория измерений
/// </summary>
public interface IMeasuresRepositoryLoader
{
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId);
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids);
	public Task<IReadOnlyList<MeasureDomain>> GetLatestMeasurementsAsync ();
	public Task<IReadOnlyList<MeasureDomain>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate);
}
