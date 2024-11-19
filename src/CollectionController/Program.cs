#pragma warning disable CA5394
using System.Globalization;
using MQTTnet;
using MQTTnet.Client;
using CollectionController.Models;
using Microsoft.Extensions.Logging;

namespace CollectionController;

internal sealed class Program
{
	private static async Task Main ()
	{
		string filePath = "configuration.json";
		List<SchedulerConfig>? configs = await ConfigLoader.LoadConfigAsync(filePath).ConfigureAwait(false);
		if (configs == null)
		{
			Console.WriteLine(Resources.EmptyConfig);
		}

		MqttFactory mqttFactory = new();
		using IMqttClient mqttClient = mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = new MqttClientOptionsBuilder()
			.WithTcpServer("localhost", 1883)
			.Build();
		_ = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).ConfigureAwait(false);
		MqttClientProvider.Client = mqttClient;

		Scheduler scheduler = new();
		await scheduler.Start(configs).ConfigureAwait(false);

		while (true)
		{
			// Input for living room light status
			Console.WriteLine(Resources.EnterLightStatus);
			string? lightInput = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(lightInput))  
			{
				Console.WriteLine(Resources.EmptyInput);
			}
			else if (bool.TryParse(lightInput, out bool isLightOn))
			{
				MqttApplicationMessage lightMessage = new MqttApplicationMessageBuilder()
					.WithTopic("home/living_room/light")
					.WithPayload(isLightOn.ToString(CultureInfo.InvariantCulture))
					.Build();

				_ = await mqttClient.PublishAsync(lightMessage, CancellationToken.None).ConfigureAwait(false);
				Console.WriteLine($"Living room light status published: {isLightOn}");
			}
			else
			{
				Console.WriteLine(Resources.InvalidInput);
			}

			// Input for door status
			Console.WriteLine(Resources.EnterDoorStatus);
			string? doorInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(lightInput))  
			{
				Console.WriteLine(Resources.EmptyInput);
			}
			else if (bool.TryParse(doorInput, out bool isDoorOpen))
			{
				MqttApplicationMessage doorMessage = new MqttApplicationMessageBuilder()
					.WithTopic("home/door")
					.WithPayload(isDoorOpen.ToString(CultureInfo.InvariantCulture))
					.Build();

				_ = await mqttClient.PublishAsync(doorMessage, CancellationToken.None).ConfigureAwait(false);
				Console.WriteLine($"Door status published: {isDoorOpen}");
			}
			else
			{
				Console.WriteLine(Resources.InvalidInput);
			}

			//input for venting
			Console.WriteLine(Resources.EnterVentingStatus);
			string? ventingInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(ventingInput))
			{
				Console.WriteLine(Resources.EmptyInput);
			}
			else if (bool.TryParse(ventingInput, out bool isVentingActive))
			{
				MqttApplicationMessage ventingMessage = new MqttApplicationMessageBuilder()
					.WithTopic("home/venting")
					.WithPayload(isVentingActive.ToString(CultureInfo.InvariantCulture))
					.Build();

				_ = await mqttClient.PublishAsync(ventingMessage, CancellationToken.None).ConfigureAwait(false);
				Console.WriteLine($"Door status published: {isVentingActive}");
			}
			else
			{
				Console.WriteLine(Resources.InvalidInput);
			}

			await Task.Delay(20000).ConfigureAwait(false);
		}
	}
}
