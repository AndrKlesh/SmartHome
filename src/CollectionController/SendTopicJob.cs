#pragma warning disable CA1303
#pragma warning disable CA1812
#pragma warning disable CA5394

using System.Globalization;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Quartz;

namespace CollectionController;
internal sealed class SendTopicJob () : IJob
{
	private readonly IMqttClient? _mqttClient = MqttClientProvider.Client;
	private readonly Random random = new();

	async Task IJob.Execute (IJobExecutionContext context)
	{
		using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
		ILogger logger = factory.CreateLogger("Program");

		string? topic = context.JobDetail.JobDataMap.GetString("Topic");
		int startValue = context.JobDetail.JobDataMap.GetInt("StartValue");
		int endValue = context.JobDetail.JobDataMap.GetInt("EndValue");
		int currentValue = startValue;
		int step = random.Next(-2, 2);
		if (currentValue + step <= endValue) // Не выходим за верхний предел
		{
			currentValue += step; // Увеличиваем
		}
		else if (currentValue + step >= startValue) // Не выходим за нижний предел
		{
			currentValue -= step; // Уменьшаем
		}

		if (_mqttClient == null)
		{
			Console.WriteLine("MQTT client is not initialized");
			return;
		}

		MqttApplicationMessage applicationMessage = new MqttApplicationMessageBuilder()
		  .WithTopic(topic)
		  .WithPayload(currentValue.ToString(CultureInfo.InvariantCulture))
		  .Build();
		_ = await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);

		string payloadString = System.Text.Encoding.UTF8.GetString(applicationMessage.PayloadSegment);
#pragma warning disable CA1848 // Использовать делегаты LoggerMessage
#pragma warning disable CA2254 // Шаблон должен быть статическим выражением
		logger.LogInformation($"Published to {topic}: {payloadString}");
	}
}
