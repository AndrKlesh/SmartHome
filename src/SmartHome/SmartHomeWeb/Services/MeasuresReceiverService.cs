using System.Text;
using MQTTnet;
using MQTTnet.Client;

namespace SmartHomeWeb.Services;

public class MeasuresReceiverService (IHostApplicationLifetime lifetime, MeasuresStorageService measuresStorageService) : IHostedService
{
	public Task StartAsync (CancellationToken cancellationToken)
	{
		lifetime.ApplicationStarted.Register(() => _ = ConfigureSubscriptions(default));
		lifetime.ApplicationStopping.Register(() => _ = UnconfigureSubscriptions(default));
		return Task.CompletedTask;
	}

	public Task StopAsync (CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task ConfigureSubscriptions (CancellationToken cancellationToken)
	{
		_mqttClient = _mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder()
										  .WithTcpServer("localhost", 1883)
										  .Build();

#pragma warning disable CA1031 // Не перехватывать исключения общих типов
		try
		{
			await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken).ConfigureAwait(false);

			_mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
			MqttClientSubscribeOptions mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
											   .WithTopicFilter("home/#")
											   .Build();
			await _mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken).ConfigureAwait(false);
		}
		// TODO: перехватывать конкретные исключения
		catch (Exception)
		{

		}
#pragma warning restore CA1031 // Не перехватывать исключения общих типов
	}
	private async Task UnconfigureSubscriptions (CancellationToken cancellationToken)
	{
		IMqttClient? mqttClient = _mqttClient;
		_mqttClient = null;
		if (mqttClient is null)
		{
			return;
		}

		mqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
		await mqttClient.DisconnectAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		mqttClient.Dispose();
	}

	private Task OnApplicationMessageReceivedAsync (MqttApplicationMessageReceivedEventArgs e)
	{
		//TODO: конвертер из mqtt во внутренние измерения
		measuresStorageService.PutMeasure(e.ApplicationMessage.Topic,
										 Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment),
										 DateTime.UtcNow);
		return Task.CompletedTask;
	}

	private IMqttClient? _mqttClient;
	private readonly MqttFactory _mqttFactory = new();
}
