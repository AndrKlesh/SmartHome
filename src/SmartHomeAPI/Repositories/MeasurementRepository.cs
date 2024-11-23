using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public class MeasurementRepository
{
	private readonly List<MeasureDomain> _measurements = new();
	private const int MaxMeasurementsPerTopic = 100;

	public async Task AddMeasurementAsync (MeasureDomain measurement)
	{
		_measurements.Add(measurement);

		List<MeasureDomain> measurementsForTopic = _measurements.Where(m => m.TopicId == measurement.TopicId).ToList();
		if (measurementsForTopic.Count > MaxMeasurementsPerTopic)
		{
			MeasureDomain? oldestMeasurement = measurementsForTopic.OrderBy(m => m.Timestamp).FirstOrDefault();
			if (oldestMeasurement != null)
			{
				_ = _measurements.Remove(oldestMeasurement);
			}
		}

		await Task.CompletedTask;
	}

	public async Task<List<MeasureDomain>> GetMeasurementsByTopicIdAsync (Guid topicId)
	{
		return await Task.FromResult(_measurements.Where(m => m.TopicId == topicId).ToList());
	}

	public async Task<List<MeasureDomain>> GetLatestMeasurementsAsync ()
	{
		return await Task.FromResult(
			_measurements
				.GroupBy(m => m.TopicId)
				.Select(g => g.OrderByDescending(m => m.Timestamp).First())
				.ToList()
		);
	}

	public async Task<List<MeasureDomain>> GetMeasurementsByTopicAndDateRangeAsync (string topicName, DateTime startDate, DateTime endDate)
	{
		return await Task.FromResult(
			_measurements
				.Where(m => m.Topic?.Name == topicName && m.Timestamp >= startDate && m.Timestamp <= endDate)
				.OrderBy(m => m.Timestamp)
				.ToList()
		);
	}
}
