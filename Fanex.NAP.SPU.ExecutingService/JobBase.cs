namespace Fanex.NAP.SPU.ExecutingService
{
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;
    using Fanex.NAP.SPU.Model;
    using System.Collections.Specialized;
    using System.Configuration;

    [DisallowConcurrentExecution]
    public class JobBase : IJobBase
    {
        public IJobDetail Job { get; set; }

        public ITrigger Trigger { get; set; }

        public IScheduler Schedule { get; set; }

        public IJobBase CreateJob(string jobName, string groupName, JobData jobData)
        {
            var jobDataMap = new JobDataMap
            {
                { "Id", jobData.Id },
                { "Name", jobData.Name },
                { "Active", jobData.Active },                
                { "FileName", jobData.FileName }                               
            };

            foreach(var executePath in jobData.ExecutePath)
            {
                jobDataMap.Add(executePath.InstanceId, executePath.Path);
            }

            var job = JobBuilder.Create(typeof(ExecuteJob))
               .WithIdentity(jobName, groupName)
               .SetJobData(jobDataMap)
               .Build();

            this.Job = job;

            return this;
        }       

        public IJobBase CreateTriggerIntervalInSeconds(string triggerName, string groupName, int interval)
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

        public IJobBase CreateTriggerWithCronSchedule(string triggerName, string groupName, string expression)
        {
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .WithCronSchedule(expression)
                .StartNow()
                .Build();

            this.Trigger = trigger;

            return this;
        }

        public IJobBase CreateSchedule()
        {
            var config = (NameValueCollection)ConfigurationManager.GetSection("quartz");

            ISchedulerFactory factory = new StdSchedulerFactory(config);

            this.Schedule = factory.GetScheduler().Result;

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

        public void Shutdown()
        {
            if (this.Schedule != null)
            {
                this.Schedule.Shutdown();
            }
        }
    }
}