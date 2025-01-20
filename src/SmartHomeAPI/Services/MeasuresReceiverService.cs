using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

internal sealed class MeasuresReceiverService (MeasuresStorageService measuresStorageService, SubscriptionService subscriptionsService) : IHostedService
{
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;
	private readonly SubscriptionService _subscriptionsService = subscriptionsService;
	private IMqttClient? _mqttClient;
	private static readonly TimeSpan _reconnectTimeout = TimeSpan.FromSeconds(5);
	private readonly MqttFactory _mqttFactory = new();

	public async Task StartAsync (CancellationToken cancellationToken)
	{
		await ConfigureSubscriptions(cancellationToken).ConfigureAwait(false);
	}

	public async Task StopAsync (CancellationToken cancellationToken)
	{
		await UnconfigureSubscriptions(cancellationToken).ConfigureAwait(false);
	}

	private async Task ConfigureSubscriptions (CancellationToken cancellationToken)
	{
		_mqttClient = _mqttFactory.CreateMqttClient();
		MqttClientOptions mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder()
			.WithTcpServer("localhost", 1883)
			.Build();

		bool isNeedRetry;
		do
		{
			isNeedRetry = false;
			try
			{
				_ = await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken).ConfigureAwait(false);

				_mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
				MqttClientSubscribeOptions mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
					.WithTopicFilter("home/#")
					.Build();
				_ = await _mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is OperationCanceledException or MqttCommunicationTimedOutException or MqttCommunicationException or TimeoutException)
			{
				isNeedRetry = true;
				Console.WriteLine($"Ошибка подключения: {ex.Message}");
				await Task.Delay(_reconnectTimeout, cancellationToken).ConfigureAwait(false);
			}
		} while (isNeedRetry);
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

		// Проверка существования топика в подписках
		SubscriptionDTO? subscription = await _subscriptionsService.GetSubscriptionByMqttTopicAsync(topic).ConfigureAwait(false);
		if (subscription is null)
		{
			Console.WriteLine($"Топик '{topic}' не добавлен пользователем. Игнорирование сообщения.");
			return;
		}

		MeasureDTO measurementDto = new()
		{
			MeasurementId = subscription.MeasurementId,
			Value = payload,
			Timestamp = timestamp
		};

		// Передаем идентификатор измерения в метод добавления
		await _measuresStorageService.AddMeasureAsync(measurementDto).ConfigureAwait(false);
	}
}
