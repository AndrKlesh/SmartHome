using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionController.Models;
public class SchedulerConfig
{
	public required string MqttTopic { get; set; }
	public int StartValue { get; set; }
	public int EndValue { get; set; }
	public required string CronExpression { get; set; }
}
