#pragma warning disable CA1515

using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис измерений
/// </summary>
/// <param name="measurementRepository">Репозиторий измерений</param>
/// <param name="subscriptionRepository">Репозиторий подписок</param>
/// <param name="measuresLinksRepository">Репозиторий ссылок на измерения</param>
public sealed class MeasuresStorageService (MeasuresRepository measurementRepository,
									 SubscriptionRepository subscriptionRepository,
									 MeasuresLinksRepository measuresLinksRepository,
									 ILogger<MeasuresStorageService> logger) : IDisposable
{
	private readonly SemaphoreSlim _newMeasuresSemaphore = new(1);
	private bool _disposed;

	/// <summary>
	/// Добавить новое измерений
	/// </summary>
	/// <param name="measurementDto">Измерение</param>
	/// <returns></returns>
	public async Task AddMeasureAsync (MeasureDTO measurementDto)
	{
		if (measurementDto == null)
		{
			logger.LogError("measurementDto был null");
		}
		else
		{
			try
			{
				logger.LogInformation("Добавление нового измерения с ID: {MeasurementId}", measurementDto.MeasurementId);

				MeasureDomain measurement = new()
				{
					MeasurementId = measurementDto.MeasurementId,
					Value = measurementDto.Value,
					Timestamp = measurementDto.Timestamp
				};

				await measurementRepository.AddMeasurementAsync(measurement).ConfigureAwait(false);

				logger.LogInformation("Измерение с ID {MeasurementId} успешно добавлено", measurementDto.MeasurementId);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Ошибка при добавлении измерения с ID: {MeasurementId}", measurementDto.MeasurementId);
			}
			finally
			{
				// TODO: Long Polling: Пределать на подписку на конкретные типы измерения
				_ = _newMeasuresSemaphore.Release();
			}
		}
	}

	/// <summary>
	/// Подписаться на последние измерения
	/// </summary>
	/// <param name="mask"></param>
	/// <returns></returns>
	public async Task<IReadOnlyList<MeasureDTO>> SubscribeToLatestMeasurementsAsync (string mask)
	{
		try
		{
			logger.LogInformation("Подписка на последние измерения с маской: {Mask}", mask);

			//TODO: Long Polling: Ожидание новых измерений
			await _newMeasuresSemaphore.WaitAsync().ConfigureAwait(false);

			IReadOnlyList<MeasureDTO> result = await GetLatestMeasurementsAsync(mask).ConfigureAwait(false);
			logger.LogInformation("Получены последние измерения по маске: {Mask}", mask);
			return result;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при подписке на последние измерения с маской: {Mask}", mask);
			throw;
		}
	}

	/// <summary>
	/// Получить последние измерения по маске.
	/// Например, получение последних измерений по маске Общие/*
	/// </summary>
	/// <param name="mask">Маска ссылок на типы измерений</param>
	/// <returns>Список последних измерений</returns>
	public async Task<IReadOnlyList<MeasureDTO>> GetLatestMeasurementsAsync (string mask)
	{
		try
		{
			logger.LogInformation("Получение последних измерений для маски: {Mask}", mask);

			IReadOnlyList<KeyValuePair<string, Guid>> measurementsLinks = await measuresLinksRepository.FindLinksByMaskAsync(mask).ConfigureAwait(false);
			IReadOnlyList<MeasureDomain> latestMeasuresDomain = await measurementRepository
				.GetLatestMeasurementsAsync(measurementsLinks.Select(l => l.Value).ToArray())
				.ConfigureAwait(false);

			List<MeasureDTO> latestMeasurementsDTO = new(latestMeasuresDomain.Count);
			foreach (MeasureDomain measure in latestMeasuresDomain)
			{
				SubscriptionDomain? subscription = await subscriptionRepository
					.GetSubscriptionByMeasurementIdAsync(measure.MeasurementId)
					.ConfigureAwait(false);

				if (subscription is null)
				{
					logger.LogWarning("Не найдена подписка для измерения с ID: {MeasurementId}", measure.MeasurementId);
					continue;
				}

				string tag = measurementsLinks.FirstOrDefault(l => l.Value == measure.MeasurementId).Key;
				int indexOfSlash = tag.LastIndexOf('/');
				indexOfSlash = indexOfSlash < 0 ? 0 : indexOfSlash + 1;
				string name = tag.Substring(indexOfSlash);

				latestMeasurementsDTO.Add(new MeasureDTO()
				{
					MeasurementId = measure.MeasurementId,
					Name = name,
					Units = subscription.Unit,
					Timestamp = measure.Timestamp,
					Value = measure.Value,
				});
			}

			logger.LogInformation("Получены {Count} последних измерений по маске: {Mask}", latestMeasurementsDTO.Count, mask);
			return latestMeasurementsDTO;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при получении последних измерений для маски: {Mask}", mask);
			throw;
		}
	}

	/// <summary>
	/// Получить историю измерения
	/// </summary>
	/// <param name="measurementId">Ид. измерения</param>
	/// <param name="startDate">Дата начала</param>
	/// <param name="endDate">Дата конца</param>
	/// <returns></returns>
	public async Task<IReadOnlyList<MeasuresHistoryDTO>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		try
		{
			logger.LogInformation("Получение истории измерений для ID: {MeasurementId} с {StartDate} по {EndDate}", measurementId, startDate, endDate);

			IReadOnlyList<MeasureDomain> measurements = await measurementRepository
				.GetMeasurementHistory(measurementId, startDate, endDate)
				.ConfigureAwait(false);

			MeasuresHistoryDTO [] history = measurements.Select(m => new MeasuresHistoryDTO
			{
				Value = m.Value,
				Timestamp = m.Timestamp
			}).ToArray();

			logger.LogInformation("Получено {Count} записей в истории измерений для ID: {MeasurementId}", history.Length, measurementId);
			return history;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при получении истории измерений для ID: {MeasurementId} с {StartDate} по {EndDate}", measurementId, startDate, endDate);
			throw;
		}
	}

	///<inheritdoc/>
	public void Dispose ()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose (bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			_newMeasuresSemaphore.Dispose();
		}

		_disposed = true;
	}
}
