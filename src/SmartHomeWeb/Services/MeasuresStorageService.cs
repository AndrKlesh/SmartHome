using SmartHomeWeb.Models;

namespace SmartHomeWeb.Services;

public class MeasuresStorageService : IMeasuresStorageService
{
	private readonly Dictionary<string, List<Measure>> _measures = new();

	public void AddMeasure (Measure measure)
	{
		if (_measures.TryGetValue(measure.Topic, out List<Measure>? existingMeasures))
		{
			Measure? existingMeasure = existingMeasures.FirstOrDefault(m => m.Timestamp == measure.Timestamp);
			if (existingMeasure != null)
			{
				existingMeasure.Value = measure.Value;
			}
			else
			{
				existingMeasures.Add(measure);
			}
		}
		else
		{
			_measures [measure.Topic] = new List<Measure> { measure };
		}
	}

	public void AddMeasures (IEnumerable<Measure> measures)
	{
		foreach (Measure measure in measures)
		{
			AddMeasure(measure);
		}
	}

	public void RemoveMeasureById (string id)
	{
		_measures.Remove(id);
	}

	public void RemoveMeasuresByTimestamp (DateTime fromDate, DateTime toDate)
	{
		foreach (List<Measure> entry in _measures.Values)
		{
			entry.RemoveAll(m => m.Timestamp >= fromDate && m.Timestamp <= toDate);
		}
	}

	public void RemoveMeasures (string id, DateTime fromDate, DateTime toDate)
	{
		if (_measures.TryGetValue(id, out List<Measure>? value))
		{
			value.RemoveAll(m => m.Timestamp >= fromDate && m.Timestamp <= toDate);
		}
	}

	public IEnumerable<Measure> GetMeasures (IEnumerable<string> ids, DateTime fromDate, DateTime toDate)
	{
		List<Measure> result = new();
		foreach (string id in ids)
		{
			if (_measures.TryGetValue(id, out List<Measure>? value))
			{
				result.AddRange(value.Where(m => m.Timestamp >= fromDate && m.Timestamp <= toDate));
			}
		}

		return result;
	}

	public Measure? GetLastMeasureById (string id)
	{
		if (_measures.TryGetValue(id, out List<Measure>? value) && value.Count != 0)
		{
			return value.OrderByDescending(m => m.Timestamp).FirstOrDefault();
		}

		return null;
	}
}
