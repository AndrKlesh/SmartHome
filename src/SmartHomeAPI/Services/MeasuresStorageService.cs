#pragma warning disable CA1515

using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис измерений
/// </summary>
public sealed class MeasuresStorageService(IServiceProvider serviceProvider,
										   SubscriptionRepository subscriptionRepository,
										   MeasuresLinksRepository measuresLinksRepository,
										   ILogger<MeasuresStorageService> logger) : IDisposable
{
	private readonly SemaphoreSlim _newMeasuresSemaphore = new(1);
	private bool _disposed;

	/// <summary>
	/// Добавить новое измерение
	/// </summary>
	public async Task AddMeasureAsync(MeasureDTO measurementDto)
	{
		if (measurementDto == null)
		{
			logger.LogError("measurementDto был null");
			throw new ArgumentNullException(nameof(measurementDto));
		}

		try
		{
			logger.LogInformation("Добавление измерения с ID: {MeasurementId}...", measurementDto.MeasurementId);

			MeasureDomain measurement = new()
			{
				MeasurementId = measurementDto.MeasurementId,
				Value = measurementDto.Value,
				Timestamp = measurementDto.Timestamp
			};

			using IServiceScope scope = serviceProvider.CreateScope();
			MeasuresRepository measurementRepository = scope.ServiceProvider.GetRequiredService<MeasuresRepository>();
			await measurementRepository.AddMeasurementAsync(measurement).ConfigureAwait(false);

			logger.LogInformation("Измерение с ID {MeasurementId} успешно добавлено", measurementDto.MeasurementId);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при добавлении измерения с ID: {MeasurementId}", measurementDto.MeasurementId);
		}
		finally
		{
			_ = _newMeasuresSemaphore.Release();
		}
	}

	/// <summary>
	/// Подписаться на последние измерения
	/// </summary>
	public async Task<IReadOnlyList<MeasureDTO>> SubscribeToLatestMeasurementsAsync(string mask)
	{
		try
		{
			logger.LogInformation("Подписка на последние измерения для маски: '{Mask}'...", mask);
			await _newMeasuresSemaphore.WaitAsync().ConfigureAwait(false);
			IReadOnlyList<MeasureDTO> result = await GetLatestMeasurementsAsync(mask).ConfigureAwait(false);
			logger.LogInformation("Получены последние измерения по маске: '{Mask}'", mask);
			return result;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при подписке на последние измерения с маской: '{Mask}'", mask);
			throw;
		}
	}

	/// <summary>
	/// Получить последние измерения по маске.
	/// </summary>
	public async Task<IReadOnlyList<MeasureDTO>> GetLatestMeasurementsAsync(string mask)
	{
		try
		{
			logger.LogInformation("Получение последних измерений для маски: '{Mask}'...", mask);
			IReadOnlyList<KeyValuePair<string, Guid>> measurementsLinks = await measuresLinksRepository.FindLinksByMaskAsync(mask).ConfigureAwait(false);

			using IServiceScope scope = serviceProvider.CreateScope();
			MeasuresRepository measurementRepository = scope.ServiceProvider.GetRequiredService<MeasuresRepository>();
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

			logger.LogInformation("Получено {Count} последних измерений по маске: '{Mask}'", latestMeasurementsDTO.Count, mask);
			return latestMeasurementsDTO;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при получении последних измерений для маски: {Mask}", mask);
			throw;
		}
	}

	/// <summary>
	/// Получить историю измерений
	/// </summary>
	public async Task<IReadOnlyList<MeasuresHistoryDTO>> GetMeasurementHistory(Guid measurementId, DateTime startDate, DateTime endDate)
	{
		try
		{
			logger.LogInformation("Получение истории измерений для ID: '{MeasurementId}' с '{StartDate}' по '{EndDate}'", measurementId, startDate, endDate);

			using IServiceScope scope = serviceProvider.CreateScope();
			MeasuresRepository measurementRepository = scope.ServiceProvider.GetRequiredService<MeasuresRepository>();
			IReadOnlyList<MeasureDomain> measurements = await measurementRepository
				.GetMeasurementHistory(measurementId, startDate, endDate)
				.ConfigureAwait(false);

			MeasuresHistoryDTO[] history = measurements.Select(m => new MeasuresHistoryDTO
			{
				Value = m.Value,
				Timestamp = m.Timestamp
			}).ToArray();

			logger.LogInformation("Получено {Count} записей в истории измерений для ID: '{MeasurementId}'", history.Length, measurementId);
			return history;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при получении истории измерений для ID: '{MeasurementId}' с '{StartDate}' по '{EndDate}'", measurementId, startDate, endDate);
			throw;
		}
	}

	///<inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
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
