#pragma warning disable CA1812

namespace CollectionController.Models;

internal sealed class SchedulerConfig
{
	public required string MqttTopic { get; set; }
	public int StartValue { get; set; }
	public int EndValue { get; set; }
	public required string CronExpression { get; set; }
}
