#pragma warning disable CA1515

using System.Diagnostics.CodeAnalysis;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис измерениц
/// </summary>
/// <param name="measurementRepository">Репозиторий измерений</param>
/// <param name="subscriptionRepository">Репозиторий подписок</param>
/// <param name="measuresLinksRepository">Репозиторий ссылок на измерения</param>
public class MeasuresStorageService (MeasurementRepository measurementRepository, 
									 SubscriptionRepository subscriptionRepository,
									 MeasuresLinksRepository measuresLinksRepository) : IDisposable
{
	///<inheritdoc/>
	public void Dispose ()
	{
		Dispose (true);
		GC.SuppressFinalize (this);
	}

	/// <summary>
	/// Добавить новое измерений
	/// </summary>
	/// <param name="measurementDto">Измерениц</param>
	/// <returns></returns>
	public async Task AddMeasureAsync (MeasureDTO measurementDto)
	{
		ArgumentNullException.ThrowIfNull(measurementDto);
		MeasureDomain measurement = new()
		{
			MeasurementId = measurementDto.MeasurementId,
			Value = measurementDto.Value,
			Timestamp = measurementDto.Timestamp
		};

		await _measurementRepository.AddMeasurementAsync(measurement).ConfigureAwait(false);
		//TODO: Long Polling: Пределать на подписку на конкретные типы измерения
		_ = _newMeasuresSemaphore.Release();
	}

	/// <summary>
	/// Подписаться на последние измерения
	/// </summary>
	/// <param name="mask"></param>
	/// <returns></returns>
	public async Task<IReadOnlyList<MeasureDTO>> SubscribeToLatestMeasurementsAsync (string mask)
	{
		//TODO: Long Polling:Ожидание новых измерений
		await _newMeasuresSemaphore.WaitAsync()
			.ConfigureAwait(false);

		IReadOnlyList<MeasureDTO> result = await GetLatestMeasurementsAsync(mask)
			.ConfigureAwait(false);
		return result;
	}

	/// <summary>
	/// Получить последние измерения по маске.
	/// Например, получение последних измерений по маске Общие/*
	/// </summary>
	/// <param name="mask">Маска ссылок на типы измерений</param>
	/// <returns>Список последних измерений</returns>
	public async Task<IReadOnlyList<MeasureDTO>> GetLatestMeasurementsAsync (string mask)
	{
		IReadOnlyList<KeyValuePair<string, Guid>> measurementsLinks = await _measuresLinksRepository.FindLinksByMaskAsync(mask)
			                                                                                        .ConfigureAwait(false);
		IReadOnlyList<MeasureDomain> latestMeasuresDomain = await _measurementRepository.GetLatestMeasurementsAsync(measurementsLinks.Select(l => l.Value)
			                                                                                                                .ToArray())
			                                                                   .ConfigureAwait(false);

		List<MeasureDTO> latestMeasurementsDTO = new(latestMeasuresDomain.Count);
		foreach (MeasureDomain measure in latestMeasuresDomain)
		{
			SubscriptionDomain? subscription = await _subscriptionRepository.GetSubscriptionByMeasurementIdAsync(measure.MeasurementId)
				                                                            .ConfigureAwait(false);
			if (subscription is null) 
			{
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
		return latestMeasurementsDTO;
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
		IReadOnlyList<MeasureDomain> measurements = await _measurementRepository.GetMeasurementHistory(measurementId, startDate, endDate).ConfigureAwait(false);

		return measurements.Select(m => new MeasuresHistoryDTO
		{
			Value = m.Value,
			Timestamp = m.Timestamp
		}).ToArray();
	}

	protected virtual void Dispose (bool disposing)
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

	private readonly MeasurementRepository _measurementRepository = measurementRepository;
	private readonly SubscriptionRepository _subscriptionRepository = subscriptionRepository;
	private readonly MeasuresLinksRepository _measuresLinksRepository = measuresLinksRepository;

	private readonly SemaphoreSlim _newMeasuresSemaphore = new(1);
	private bool _disposed;
}
