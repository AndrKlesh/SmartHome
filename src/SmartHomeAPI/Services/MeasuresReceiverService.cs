using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

public class MeasuresReceiverService (IHostApplicationLifetime lifetime, MeasuresStorageService measuresStorageService) : IHostedService
{
	private readonly IHostApplicationLifetime _lifetime = lifetime;
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;
	private IMqttClient? _mqttClient;
	private readonly MqttFactory _mqttFactory = new();

	public Task StartAsync (CancellationToken cancellationToken)
	{
		_ = _lifetime.ApplicationStarted.Register(() => _ = ConfigureSubscriptions(cancellationToken));
		_ = _lifetime.ApplicationStopping.Register(() => _ = UnconfigureSubscriptions(cancellationToken));
		return Task.CompletedTask;
	}

	public Task StopAsync (CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task ConfigureSubscriptions (CancellationToken cancellationToken)
	{
		_mqttClient = _mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

		try
		{
			_ = await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken).ConfigureAwait(false);

			_mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
			MqttClientSubscribeOptions mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("home/#").Build();
			_ = await _mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken).ConfigureAwait(false);
		}
		catch (OperationCanceledException ex)
		{
			Console.WriteLine($"Операция была отменена: {ex.Message}");
		}
		catch (MqttCommunicationTimedOutException ex)
		{
			Console.WriteLine($"Тайм-аут при подключении к MQTT брокеру: {ex.Message}");
			// TODO: повторное подключение
		}
		catch (MqttCommunicationException ex)
		{
			Console.WriteLine($"Ошибка коммуникации с MQTT брокером: {ex.Message}");
			// TODO: восстановить подключение
		}
		catch (TimeoutException ex)
		{
			Console.WriteLine($"Произошел тайм-аут операции: {ex.Message}");
			// TODO: повторное подключение
		}
	}

	private async Task UnconfigureSubscriptions (CancellationToken cancellationToken)
	{
		IMqttClient? mqttClient = Interlocked.Exchange(ref _mqttClient, null);
		if (mqttClient is null)
		{
			return;
		}

		mqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
		await mqttClient.DisconnectAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		mqttClient.Dispose();
	}

	private async Task OnApplicationMessageReceivedAsync (MqttApplicationMessageReceivedEventArgs e)
	{
		string topic = e.ApplicationMessage.Topic;
		string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
		DateTime timestamp = DateTime.UtcNow;

		MeasureDTO measurementDto = new()
		{
			TopicName = topic,
			Value = payload,
			Timestamp = timestamp
		};

		// Сохранение данных через сервис
		await _measuresStorageService.AddMeasureAsync(measurementDto);
	}
}
