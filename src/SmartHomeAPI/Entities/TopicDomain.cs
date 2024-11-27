namespace SmartHomeAPI.Entities;

public class TopicDomain
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsFavourite { get; set; }
	public ICollection<MeasureDomain> Measurements { get; set; } = new List<MeasureDomain>();
}
