#pragma warning disable CA1002
#pragma warning disable CA1822 
using CollectionController.Models;
using MQTTnet.Client;
using Quartz;
using Quartz.Impl;

namespace CollectionController;
public sealed class Scheduler ()
{
	public async Task Start(List<SchedulerConfig>? configs)
	{
		IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false);
		await scheduler.Start().ConfigureAwait(false);

		if (configs == null)
		{
			Console.WriteLine(Resources.EmptyConfig);
			return;
		}

		foreach (SchedulerConfig config in configs)
		{
			IJobDetail job = JobBuilder.Create<SendTopicJob>()
				.UsingJobData("Topic", config.MqttTopic) 
				.UsingJobData("StartValue", config.StartValue)
				.UsingJobData("EndValue", config.EndValue)
				.Build();

			ITrigger trigger = TriggerBuilder.Create()
					.WithIdentity(config.MqttTopic + "trigger1", "group1")
					.WithCronSchedule(config.CronExpression)
					.Build();

			_ = await scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
		}
	}
}
