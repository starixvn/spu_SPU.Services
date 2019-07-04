namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.NAP.SPU.ExecutingService.Jobs;
    using Fanex.NAP.FS.Model;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;
    using Fanex.NAP.SPU.Model;

    [DisallowConcurrentExecution]
    public class JobBase : IJobBase
    {
        public IJobDetail Job { get; set; }

        public ITrigger Trigger { get; set; }

        public IScheduler Schedule { get; set; }

        public IJobBase CreateJob(string jobName, string groupName, JobData jobData)
        {            
            var job = JobBuilder.Create(typeof(ExecuteJob))
               .WithIdentity(jobName, groupName)
               .UsingJobData("Active", jobData.Active)
               .UsingJobData("Path", jobData.Path)
               .Build();

            this.Job = job;

            return this;
        }       

        public IJobBase CreateTrigger(string triggerName, string groupName, int interval)
        {
            var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerName, groupName)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(interval)
                .RepeatForever())
            .Build();

            this.Trigger = trigger;

            return this;
        }        

        public IJobBase CreateSchedule()
        {
            this.Schedule = StdSchedulerFactory.GetDefaultScheduler().Result;

            if (this.Job != null && this.Trigger != null)
            {
                this.Schedule.ScheduleJob(this.Job, this.Trigger);
            }            

            return this;
        }

        public IJobBase CreateListenerJob()
        {
            var listener = new ListenerJob(this.Job.Key.Name);
            IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(this.Job.Key);
            this.Schedule.ListenerManager.AddJobListener(listener, matcher);

            return this;
        }

        public void Run()
        {
            if (this.Schedule != null)
            {
                this.Schedule.Start();
            }
        }              
     
        public IJobBase DeleteJob(string jobName, string groupName)
        {
            var jobKey = new JobKey(jobName, groupName);

            if(this.Schedule.CheckExists(jobKey).Result)
            {
                this.Schedule.DeleteJob(jobKey);
            }

            return this;
        }
    }
}