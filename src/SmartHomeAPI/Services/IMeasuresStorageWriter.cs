#pragma warning disable CA1515

using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

/// <summary>
/// Интерфейс записи для сервиса хранения измерений
/// </summary>
public interface IMeasuresStorageWriter
{
	public Task AddMeasureAsync (MeasureDTO measurementDto);
}
