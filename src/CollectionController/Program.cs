#pragma warning disable CA1303
#pragma warning disable CA5394

using System.Globalization;
using CollectionController.Models;
using MQTTnet;
using MQTTnet.Client;

namespace CollectionController;

internal sealed class Program
{
	private static async Task Main ()
	{
		string filePath = "configuration.json";
		List<SchedulerConfig>? configs = await ConfigLoader.LoadConfigAsync(filePath).ConfigureAwait(false);
		if (configs == null)
		{
			Console.WriteLine("Your configuration file is empty");
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
			Console.WriteLine("Enter the living room light status (true/false): ");
			string? lightInput = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(lightInput))
			{
				Console.WriteLine("Input cannot be null or empty");
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
				Console.WriteLine("Invalid input. Enter true or false");
			}

			// Input for door status
			Console.WriteLine("Enter the door status (true/false): ");
			string? doorInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(lightInput))
			{
				Console.WriteLine("Input cannot be null or empty");
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
				Console.WriteLine("Invalid input. Please enter true or false");
			}

			// Input for venting
			Console.WriteLine("Enter the venting status(true/false): ");
			string? ventingInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(ventingInput))
			{
				Console.WriteLine("Input cannot be null or empty");
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
				Console.WriteLine("Invalid input. Please enter true or false");
			}

			await Task.Delay(20000).ConfigureAwait(false);
		}
	}
}
