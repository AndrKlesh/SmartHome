#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

/// <summary>
/// Интерфейс записи для репозитория измерений
/// </summary>
public interface IMeasuresRepositoryWriter
{
	public Task AddMeasurementAsync (MeasureDomain measurement);
}
