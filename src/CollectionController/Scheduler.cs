using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionController.Models;
using Quartz;
using Quartz.Impl;

namespace CollectionController;
public static class Scheduler
{
	public static async Task Start(List<SchedulerConfig>? configs)
	{
		IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false);
		await scheduler.Start().ConfigureAwait(false);

		if (configs == null)
		{
			await Task.Run(() => Console.WriteLine("MQTT client is not initialized.")).ConfigureAwait(false);
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
