#pragma warning disable CA1303
#pragma warning disable CA1812
#pragma warning disable CA5394

using System.Globalization;
using Microsoft.Extensions.Logging;
using MQTTnet;
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

#pragma warning disable CA1848
#pragma warning disable CA2253
		logger.LogInformation("Published to {0}: {1}", topic, currentValue);
#pragma warning restore CA2253
#pragma warning restore CA1848
	}
}
