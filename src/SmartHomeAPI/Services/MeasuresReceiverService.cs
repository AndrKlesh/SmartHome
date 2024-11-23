using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

public class MeasuresReceiverService (MeasuresStorageService measuresStorageService, SubscriptionsService subscriptionsService) : IHostedService
{
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;
	private readonly SubscriptionsService _subscriptionsService = subscriptionsService;
	private IMqttClient? _mqttClient;
	private readonly MqttFactory _mqttFactory = new();

	public async Task StartAsync (CancellationToken cancellationToken)
	{
		await ConfigureSubscriptions(cancellationToken);
	}

	public async Task StopAsync (CancellationToken cancellationToken)
	{
		await UnconfigureSubscriptions(cancellationToken);
	}

	private async Task ConfigureSubscriptions (CancellationToken cancellationToken)
	{
		_mqttClient = _mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder()
			.WithTcpServer("localhost", 1883)
			.Build();

		try
		{
			_ = await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken).ConfigureAwait(false);

			_mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
			MqttClientSubscribeOptions mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
				.WithTopicFilter("home/#")
				.Build();
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

		// Проверяем, существует ли подписка для данного топика
		SubscriptionDomain? subscription = await _subscriptionsService.GetSubscriptionByMqttTopicAsync(topic);
		if (subscription == null)
		{
			Console.WriteLine($"Topic {topic} not found in subscription list");
			return;
		}

		MeasureDTO measurementDto = new()
		{
			TopicName = topic,
			Value = payload,
			Timestamp = timestamp
		};

		// Передаем идентификатор измерения в метод добавления
		await _measuresStorageService.AddMeasureAsync(measurementDto, subscription.MeasurementId);
	}
}
