using System.Text;
using MQTTnet;
using MQTTnet.Exceptions;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Services;

/// <summary>
/// Сервис приема измерений из mqtt-брокера
/// </summary>
/// <param name="measuresStorageService">Сервис измерений</param>
/// <param name="subscriptionsService">Сервис подписок на измерения</param>
internal sealed class MeasuresReceiverService : IHostedService
{
	public MeasuresReceiverService (MeasuresStorageService measuresStorageService,
									SubscriptionService subscriptionsService)
	{
		_mqttFactory = new();
		_measuresStorageService = measuresStorageService;
		_subscriptionsService = subscriptionsService;
		_mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder()
										 .WithTcpServer("localhost", 1883)
										 .Build();
	}

	public async Task StartAsync (CancellationToken cancellationToken)
	{
		await ConfigureSubscriptions(cancellationToken).ConfigureAwait(false);
	}

	public async Task StopAsync (CancellationToken cancellationToken)
	{
		await UnconfigureSubscriptions(cancellationToken).ConfigureAwait(false);
	}

	private Task ConfigureSubscriptions (CancellationToken cancellationToken)
	{
		IMqttClient mqttClient = _mqttFactory.CreateMqttClient();
		mqttClient.DisconnectedAsync += OnDisconnectedAsync;

		_ = TryConnectAsync(mqttClient, cancellationToken);

		_mqttClient = mqttClient;
		return Task.CompletedTask;
	}

	private async Task OnDisconnectedAsync (MqttClientDisconnectedEventArgs arg)
	{
		Console.WriteLine("Подключение с mqtt-брокером разорвано");
		IMqttClient? mqttClient = _mqttClient;
		if (mqttClient is null)
		{
			return;
		}

		await Task.Delay(_reconnectTimeout).ConfigureAwait(false);
		await TryConnectAsync(mqttClient, default).ConfigureAwait(false);
	}
	private async Task TryConnectAsync (IMqttClient mqttClient, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine("Выполняется попытка подключения к mqtt-брокеру");
			_ = await mqttClient.ConnectAsync(_mqttClientOptions, cancellationToken).ConfigureAwait(false);

			mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
			MqttClientSubscribeOptions mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
				.WithTopicFilter("home/#")
				.Build();
			_ = await mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken).ConfigureAwait(false);
			_ = Interlocked.Exchange(ref _mqttClient, mqttClient);
			Console.WriteLine("Подключение к брокеру mqtt успешно выполнено.");
		}
		catch (Exception ex) when (ex is OperationCanceledException or MqttCommunicationTimedOutException or MqttCommunicationException or TimeoutException)
		{
			Console.WriteLine($"Ошибка подключения к брокеру mqtt: {ex.Message}");
		}
	}

	private async Task UnconfigureSubscriptions (CancellationToken cancellationToken)
	{
		IMqttClient? mqttClient = Interlocked.Exchange(ref _mqttClient, null);
		if (mqttClient is null)
		{
			return;
		}

		mqttClient.DisconnectedAsync -= OnDisconnectedAsync;
		mqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
		await mqttClient.DisconnectAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		mqttClient.Dispose();
	}

	private async Task OnApplicationMessageReceivedAsync (MqttApplicationMessageReceivedEventArgs e)
	{
		string topic = e.ApplicationMessage.Topic;
		string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
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

	private readonly MeasuresStorageService _measuresStorageService;
	private readonly SubscriptionService _subscriptionsService;

	private IMqttClient? _mqttClient;
	private readonly MqttClientFactory _mqttFactory;
	private readonly MqttClientOptions _mqttClientOptions;

	private static readonly TimeSpan _reconnectTimeout = TimeSpan.FromSeconds(5);
}
