namespace SmartHomeAPI.Entities;

internal class TopicDomain
{
	internal Guid Id { get; set; }
	internal string Name { get; set; } = string.Empty;
	internal bool IsFavourite { get; set; }
	internal ICollection<MeasureDomain> Measurements { get; set; } = new List<MeasureDomain>();
}
