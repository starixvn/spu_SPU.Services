namespace Fanex.NAP.SPU.Main.Provider
{
    using Quartz;
    using Quartz.Impl;

    public class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler CreateScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;

            var job = JobBuilder.Create<ExecuteJob>()
                .WithIdentity("localJob", "default")
                .Build();

            var trigger = TriggerBuilder.Create()
               .WithIdentity("trigger1", "default")
               .ForJob(job)
               .StartNow()
               .WithCronSchedule("0 /1 * ? * *")
               .Build();

            scheduler.ScheduleJob(job, trigger);

            scheduler.Start();

            return scheduler;
        }
    }
}
