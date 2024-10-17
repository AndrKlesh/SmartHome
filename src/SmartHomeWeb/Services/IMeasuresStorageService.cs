using SmartHomeWeb.Models;

namespace SmartHomeWeb.Services;

public interface IMeasuresStorageService
{
	void AddMeasure (Measure measure);

	void AddMeasures (IEnumerable<Measure> measures);

	void RemoveMeasureById (string id);

	void RemoveMeasuresByTimestamp (DateTime fromDate, DateTime toDate);

	void RemoveMeasures (string id, DateTime fromDate, DateTime toDate);

	IEnumerable<Measure> GetMeasures (IEnumerable<string> ids, DateTime fromDate, DateTime toDate);

	Measure? GetLastMeasureById (string id);
}
