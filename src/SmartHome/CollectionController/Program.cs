#pragma warning disable CA5394

using System.Globalization;
using MQTTnet;
using MQTTnet.Client;

namespace CollectionController;

internal sealed class Program
{
	private static async Task Main ()
	{
		MqttFactory mqttFactory = new();

		using IMqttClient mqttClient = mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = new MqttClientOptionsBuilder()
			.WithTcpServer("localhost", 1883)
			.Build();

		_ = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).ConfigureAwait(false);

		Random random = new();
		while (!Console.KeyAvailable)
		{
			MqttApplicationMessage applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/door")
			  .WithPayload(random.Next(0, 2).ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/outside/temperature")
			  .WithPayload(random.Next(5, 12).ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/living_room/temperature")
			  .WithPayload(random.Next(14, 26).ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			applicationMessage = new MqttApplicationMessageBuilder()
			.WithTopic("home/living_room/lighting")
			.WithPayload(random.Next(0, 2).ToString(CultureInfo.InvariantCulture))
			.Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/bathroom/cold_water_temp")
			  .WithPayload(random.Next(5, 18).ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/bathroom/hot_water_temp")
			  .WithPayload(random.Next(30, 65).ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			int airHumidity = random.Next(25, 96);
			applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/bathroom/air_humidity")
			  .WithPayload(airHumidity.ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic("home/bathroom/venting")
			  .WithPayload((airHumidity > 85 ? 1 : 0).ToString(CultureInfo.InvariantCulture))
			  .Build();
			_ = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

			await Task.Delay(500).ConfigureAwait(false);
		}

		await mqttClient.DisconnectAsync().ConfigureAwait(false);
	}
}
