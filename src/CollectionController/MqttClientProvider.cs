using MQTTnet;

namespace CollectionController;

internal static class MqttClientProvider
{
	public static IMqttClient? Client { get; set; }
	public static void Dispose ()
	{
		Client?.Dispose();
	}
}
