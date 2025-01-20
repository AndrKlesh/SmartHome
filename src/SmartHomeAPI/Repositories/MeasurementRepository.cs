#pragma warning disable CA1515

using System.Globalization;
using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public class MeasurementRepository
{
	private readonly List<MeasureDomain> _measurements = new();
	private const int MaxMeasurementsPerTopic = 100;

	public async Task AddMeasurementAsync (MeasureDomain measurement)
	{
		_measurements.Add(measurement);

		List<MeasureDomain> measurementsForTopic = _measurements.Where(m => m.MeasurementId == measurement.MeasurementId).ToList();
		if (measurementsForTopic.Count > MaxMeasurementsPerTopic)
		{
			MeasureDomain? oldestMeasurement = measurementsForTopic.OrderBy(m => m.Timestamp).FirstOrDefault();
			if (oldestMeasurement != null)
			{
				_ = _measurements.Remove(oldestMeasurement);
			}
		}

		await Task.CompletedTask.ConfigureAwait(false);
	}

	public async Task<List<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid measurementId)
	{
		return await Task.FromResult(_measurements.Where(m => m.MeasurementId == measurementId).ToList()).ConfigureAwait(false);
	}

	public async Task<List<MeasureDomain>> GetLatestMeasurementsAsync (IReadOnlyList<Guid> ids)
	{
		return await Task.FromResult(
			_measurements
			    .Where(m => ids.Contains(m.MeasurementId))
				.GroupBy(m => m.MeasurementId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToList()
		).ConfigureAwait(false);
	}

	public async Task<List<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		return await Task.FromResult(
			_measurements
				.GroupBy(m => m.MeasurementId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToList()
		).ConfigureAwait(false);
	}

	public async Task<List<MeasureDomain>> GetMeasurementHistory (Guid measurementId, DateTime startDate, DateTime endDate)
	{
		return await Task.FromResult(
			_measurements
				.Where(m => m.MeasurementId == measurementId && m.Timestamp >= startDate && m.Timestamp <= endDate)
				.OrderBy(m => m.Timestamp)
				.ToList()
		).ConfigureAwait(false);
	}
}
