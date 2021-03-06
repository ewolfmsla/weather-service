﻿
using Quartz;
using Quartz.Impl;
using Serilog;
using System;
using System.Threading.Tasks;

namespace WeatherService.Scheduled
{
    public class SchedulerJob 
    {

        public static async Task RunAsync(AerisJobParams aerisJobParams)
        //public static async Task RunAsync()
        {
            try
            {
                IScheduler scheduler;
                var schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.Context.Put("aerisJobParams", aerisJobParams);
                scheduler.Start().Wait();

                //int ScheduleIntervalInMinute = 1;//job will run every minute
                JobKey aerisKey = JobKey.Create("AerisJob");
                JobKey regressionKey = JobKey.Create("RegressionJob");

                IJobDetail aerisJob = JobBuilder.Create<AerisJob>().WithIdentity(aerisKey).Build();
                IJobDetail regressionJob = JobBuilder.Create<RegressionJob>().WithIdentity(regressionKey).Build();

                ITrigger aerisTrigger = TriggerBuilder.Create()
                    .WithIdentity("AerisTrigger")
                    .StartNow()
                    //.WithSimpleSchedule(x => x.WithIntervalInMinutes(ScheduleIntervalInMinute).RepeatForever())
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).WithRepeatCount(0))
                    .Build();

                DateTimeOffset aerisJobFinished = await scheduler.ScheduleJob(aerisJob, aerisTrigger);

                ITrigger regressionTrigger = TriggerBuilder.Create()
                    .WithIdentity("RegressionTrigger")
                    .StartAt(aerisJobFinished)
                    //.WithSimpleSchedule(x => x.WithIntervalInMinutes(ScheduleIntervalInMinute).RepeatForever())
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).WithRepeatCount(0))
                    .Build();

                await scheduler.ScheduleJob(regressionJob, regressionTrigger);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Log.Error(e.StackTrace);
            }
        }
    }
}
