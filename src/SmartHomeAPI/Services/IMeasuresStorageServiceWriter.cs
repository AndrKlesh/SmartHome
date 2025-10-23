#pragma warning disable CA1515

using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

/// <summary>
/// Интерфейс записи для сервиса хранения измерений
/// </summary>
public interface IMeasuresStorageServiceWriter
{
	public Task AddMeasureAsync (MeasureDTO measurementDto);
}
