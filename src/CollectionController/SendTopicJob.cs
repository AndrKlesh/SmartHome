#pragma warning disable CA5394
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using MQTTnet;
using System.Globalization;
using MQTTnet.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CollectionController;
public sealed class SendTopicJob () : IJob
{
	private readonly IMqttClient? _mqttClient = MqttClientProvider.Client;
	private readonly Random random = new();

	[Obsolete]
	async Task IJob.Execute (IJobExecutionContext context)
	{
		if (_mqttClient == null)
		{
			await Task.Run(() => Console.WriteLine("MQTT client is not initialized.")).ConfigureAwait(false);
			return;
		}

		string? topic = context.JobDetail.JobDataMap.GetString("Topic");
		int startValue = context.JobDetail.JobDataMap.GetInt("StartValue");
		int endValue = context.JobDetail.JobDataMap.GetInt("EndValue");
		int currentValue = startValue;
		int step = random.Next(-2, 2);
		if (currentValue + step <= endValue)//не выходим за верхний предел
		{
			currentValue += step; // Увеличиваем
		}
		else if (currentValue + step >= startValue)//не выходим за нижний предел
		{
			currentValue -= step; // Уменьшаем
		}
		

		MqttApplicationMessage applicationMessage = new MqttApplicationMessageBuilder()
			  .WithTopic(topic)
			  .WithPayload(currentValue.ToString(CultureInfo.InvariantCulture))
			  .Build();
		_ = await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None).ConfigureAwait(false);
		
		//вывод в консоль
		string payloadString = System.Text.Encoding.UTF8.GetString(applicationMessage.Payload);
		await Task.Run(() => Console.WriteLine($"Published to {topic}: {payloadString}")).ConfigureAwait(false);
	}
}
