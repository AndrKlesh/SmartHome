using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client;

namespace CollectionController;
public static class MqttClientProvider
{
	public static IMqttClient? Client { get; set; }
}
