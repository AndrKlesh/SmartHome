using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using SmartHomeWeb.Models;

namespace SmartHomeWeb.Services;

public class MeasuresReceiverService (IHostApplicationLifetime lifetime, IMeasuresStorageService measuresStorageService) : IHostedService
{
	private readonly IHostApplicationLifetime _lifetime = lifetime;
	private readonly IMeasuresStorageService _measuresStorageService = measuresStorageService;
	private IMqttClient? _mqttClient;
	private readonly MqttFactory _mqttFactory = new();

	public Task StartAsync (CancellationToken cancellationToken)
	{
		_lifetime.ApplicationStarted.Register(() => _ = ConfigureSubscriptions(cancellationToken));
		_lifetime.ApplicationStopping.Register(() => _ = UnconfigureSubscriptions(cancellationToken));
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
			await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken).ConfigureAwait(false);

			_mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
			MqttClientSubscribeOptions mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("home/#").Build();
			await _mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken).ConfigureAwait(false);
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

	private Task OnApplicationMessageReceivedAsync (MqttApplicationMessageReceivedEventArgs e)
	{
		// Конвертация MQTT-сообщения в модель Measure
		string topic = e.ApplicationMessage.Topic;
		string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
		DateTime timestamp = DateTime.UtcNow;

		// Сохранение измерения в хранилище
		Measure measure = new(topic, payload, timestamp);
		_measuresStorageService.AddMeasure(measure);

		return Task.CompletedTask;
	}
}
