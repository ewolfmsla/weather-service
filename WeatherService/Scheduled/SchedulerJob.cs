﻿
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace WeatherService.Scheduled
{
    public class SchedulerJob
    {
        public static async Task RunAsync()
        {
            try
            {
                IScheduler scheduler;
                var schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.Start().Wait();

                int ScheduleIntervalInMinute = 1;//job will run every minute
                JobKey jobKey = JobKey.Create("DemoJob1");

                IJobDetail job = JobBuilder.Create<MyJob>().WithIdentity(jobKey).Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("JobTrigger")
                    .StartNow()
                    //.WithSimpleSchedule(x => x.WithIntervalInMinutes(ScheduleIntervalInMinute).RepeatForever())
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}