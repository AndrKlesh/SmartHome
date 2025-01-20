#pragma warning disable CA1515

using System.Diagnostics.CodeAnalysis;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

public class MeasuresStorageService (MeasurementRepository measurementRepository, 
									 SubscriptionRepository subscriptionRepository,
									 MeasuresLinksRepository measuresLinksRepository)
{
	private readonly MeasurementRepository _measurementRepository = measurementRepository;
	private readonly SubscriptionRepository _subscriptionRepository = subscriptionRepository;
	private readonly MeasuresLinksRepository _measuresLinksRepository = measuresLinksRepository;

	public async Task AddMeasureAsync ([NotNull]MeasureDTO measurementDto)
	{
		MeasureDomain measurement = new()
		{
			MeasurementId = measurementDto.MeasurementId,
			Value = measurementDto.Value,
			Timestamp = measurementDto.Timestamp
		};

		await _measurementRepository.AddMeasurementAsync(measurement).ConfigureAwait(false);
	}

	public async Task<List<MeasureDTO>> GetLatestMeasurementsAsync (string mask)
	{
		IReadOnlyList<KeyValuePair<string, Guid>> measurementsLinks = await _measuresLinksRepository.FindLinksByMaskAsync(mask)
			                                                                                        .ConfigureAwait(false);
		List<MeasureDomain> latestMeasuresDomain = await _measurementRepository.GetLatestMeasurementsAsync(measurementsLinks.Select(l => l.Value)
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

	public async Task<List<MeasuresHistoryDTO>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		List<MeasureDomain> measurements = await _measurementRepository.GetMeasurementHistory(measurementId, startDate, endDate).ConfigureAwait(false);

		return measurements.Select(m => new MeasuresHistoryDTO
		{
			Value = m.Value,
			Timestamp = m.Timestamp
		}).ToList();
	}
}
