#pragma warning disable CA5394

using System.Globalization;
using MQTTnet;
using MQTTnet.Client;
using CollectionController.Models;
using System.Text.Json;
using System;
using System.Diagnostics.Metrics;

namespace CollectionController;

internal sealed class Program
{
	private static async Task Main ()
	{
		string filePath = "appsettings.json";
		List<SchedulerConfig>? configs = await ConfigLoader.LoadConfigAsync(filePath).ConfigureAwait(false);
		if (configs == null)
		{
			return; // Или обработка ошибки
		}

		MqttFactory mqttFactory = new();
		using IMqttClient mqttClient = mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = new MqttClientOptionsBuilder()
			.WithTcpServer("localhost", 1883)
			.Build();
		_ = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).ConfigureAwait(false);
		MqttClientProvider.Client = mqttClient;

		await Scheduler.Start(configs).ConfigureAwait(false);
		while (true)
		{
			// Input for living room light status
			await Task.Run(() => Console.WriteLine("Enter the living room light status (true/false): ")).ConfigureAwait(false);

			string? lightInput = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(lightInput))  
			{
				await Task.Run(()=>Console.WriteLine("Input cannot be null or empty.")).ConfigureAwait(false);
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
				await Task.Run(()=>Console.WriteLine("Invalid input. Please enter true or false.")).ConfigureAwait(false);
			}

			// Input for door status
			await Task.Run(() => Console.WriteLine("Enter the door status (true/false): ")).ConfigureAwait(false);
			string? doorInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(lightInput))  
			{
				await Task.Run(() => Console.WriteLine("Input cannot be null or empty.")).ConfigureAwait(false);
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
				await Task.Run(() => Console.WriteLine("Invalid input.Please enter true or false.")).ConfigureAwait(false);
			}

			await Task.Run(() => Console.WriteLine("Enter the venting status(true/false): ")).ConfigureAwait(false);
			string? ventingInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(ventingInput))
			{
				await Task.Run(() => Console.WriteLine("Input cannot be null or empty.")).ConfigureAwait(false);
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
				await Task.Run(() => Console.WriteLine("Invalid input.Please enter true or false.")).ConfigureAwait(false);
			}

			await Task.Delay(20000).ConfigureAwait(false);
		}

		
		//await mqttClient.DisconnectAsync().ConfigureAwait(false);
	}
}
