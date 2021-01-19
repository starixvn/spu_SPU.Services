namespace Fanex.NAP.SPU.Main.Provider
{
    using Quartz;
    using Quartz.Impl;

    public interface ISchedulerProvider
    {
        IScheduler CreateScheduler();

        bool UpdateJobs();
    }

    public class SchedulerProvider : ISchedulerProvider
    {
        private IScheduler _scheduler;

        public SchedulerProvider() { }

        public IScheduler CreateScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;

            var job = JobBuilder.Create<ExecuteJob>()
                .WithIdentity("localJob", "default")
                .Build();

            var trigger = TriggerBuilder.Create()
               .WithIdentity("trigger1", "default")
               .ForJob(job)
               .StartNow()
               .WithCronSchedule("0 /1 * ? * *")
               .Build();

            _scheduler.ScheduleJob(job, trigger);

            _scheduler.Start();

            return _scheduler;
        }

        public bool UpdateJobs()
        {
            AddNewJobs();

            UpdateJobExisteds();

            return true;
        }

        private void AddNewJobs()
        {
            var job = JobBuilder.Create<ExecuteJob>()
                .WithIdentity("newJob", "default")
                .Build();

            var trigger = TriggerBuilder.Create()
               .WithIdentity("trigger2", "default")
               .ForJob(job)
               .StartNow()
               .WithCronSchedule("0 /1 * ? * *")
               .Build();

            _scheduler.ScheduleJob(job, trigger);
        }

        private void UpdateJobExisteds()
        {

        }
    }
    
}
