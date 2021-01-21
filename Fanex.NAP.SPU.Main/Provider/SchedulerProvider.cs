namespace Fanex.NAP.SPU.Main.Provider
{
    using Fanex.NAP.SPU.Main.Models;
    using Quartz;
    using Quartz.Impl;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISchedulerProvider
    {
        IScheduler CreateScheduler();

        bool UpdateJobs(SchedulerModel schedulerModel);
    }

    public class SchedulerProvider : ISchedulerProvider
    {
        private IScheduler _scheduler;
        private SchedulerModel _currentSchedulerModel { get; set; }

        public SchedulerProvider() 
        {
            _currentSchedulerModel = new SchedulerModel();
        }

        public IScheduler CreateScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;

            //var job = JobBuilder.Create<ExecuteJob>()
            //    .WithIdentity("localJob", "default")
            //    .Build();

            //var trigger = TriggerBuilder.Create()
            //   .WithIdentity("trigger1", "default")
            //   .ForJob(job)
            //   .StartNow()
            //   .WithCronSchedule("0 /1 * ? * *")
            //   .Build();

            //_scheduler.ScheduleJob(job, trigger);

            _scheduler.Start();

            return _scheduler;
        }

        public bool UpdateJobs(SchedulerModel updateSchedulerModel)
        {
            if (updateSchedulerModel == null)
            {
                return true;
            }

            var currentJobs = _currentSchedulerModel.Jobs;

            var newJobList = new List<JobModel>();

            foreach (var job in updateSchedulerModel.Jobs)
            {
                var existedJob = currentJobs.Any(x => x.JobName == job.JobName);

                if (!existedJob)
                {
                    newJobList.Add(job);
                }
            }

            AddNewJobs(newJobList);

            return true;
        }

        private void AddNewJobs(IEnumerable<JobModel> newJobList)
        {
            foreach (var jobItem in newJobList)
            {
                var job = JobBuilder.Create<ExecuteJob>()
                   .WithIdentity(jobItem.JobName, "default")
                   .Build();

                foreach (var triggerItem in jobItem.Triggers)
                {
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerItem.TriggerName, "default")
                        .ForJob(job)
                        .StartNow()
                        .WithCronSchedule("0 /1 * ? * *")
                        .Build();

                    _scheduler.ScheduleJob(job, trigger);
                }
            }
        }
       
    }
    
}
