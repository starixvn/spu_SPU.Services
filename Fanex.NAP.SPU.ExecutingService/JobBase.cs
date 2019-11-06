namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.NAP.SPU.Model;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;

    public class JobBase : IJobBase
    {
        public IJobDetail Job { get; set; }

        public ITrigger Trigger { get; set; }

        public IScheduler Schedule { get; set; }

        public string JobName { get; set; }

        public string GroupName { get; set; }

        public string TriggerName { get; set; }


        public IJobBase CreateJob<T>(JobData jobData)
        {
            var jobDataMap = new JobDataMap
            {
                { "Id", jobData.Id },
                { "Name", jobData.Name },
                { "Active", jobData.Active },
                { "FileName", jobData.FileName },
                { "Action", jobData.Action }
            };

            foreach (var executePath in jobData.ExecutePath)
            {
                jobDataMap.Add(executePath.InstanceId, executePath.Path);
            }

            var job = JobBuilder.Create(typeof(T))
               .WithIdentity(this.JobName, this.GroupName)
               .SetJobData(jobDataMap)
               .Build();

            this.Job = job;

            return this;
        }

        public IJobBase CreateTriggerIntervalInSeconds(int interval)
        {
            var startTime = DateTimeOffset.Now.AddSeconds(5);

            var trigger = TriggerBuilder.Create()
            .WithIdentity(this.TriggerName, this.GroupName)
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(interval)
                .RepeatForever())
            .StartAt(startTime)
            .Build();

            this.Trigger = trigger;

            return this;
        }

        public IJobBase CreateTriggerWithCronSchedule(string expression)
        {
            var startTime = DateTimeOffset.Now.AddSeconds(5);

            var trigger = TriggerBuilder.Create()
                .WithIdentity(this.TriggerName, this.GroupName)
                .WithCronSchedule(expression)
                .StartAt(startTime)
                .Build();

            this.Trigger = trigger;

            return this;
        }

        public IJobBase GetSchedule()
        {
            var config = (NameValueCollection)ConfigurationManager.GetSection("quartz");

            ISchedulerFactory factory = new StdSchedulerFactory(config);

            this.Schedule = factory.GetScheduler().Result;

            if (!this.Schedule.IsStarted)
            {
                this.Schedule.Start().Wait();
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

        public IJobBase DeleteJob(string jobName)
        {
            var jobKey = new JobKey(jobName);

            this.Schedule.DeleteJob(jobKey);

            return this;
        }

        public void Shutdown()
        {
            if (this.Schedule != null)
            {
                this.Schedule.Shutdown();
            }
        }        

        public void Run()
        {
            var currentJobs = this.Schedule.GetJobKeys(GroupMatcher<JobKey>.GroupContains(this.GroupName)).Result;

            var isExistingJob = currentJobs.Any(x => x.Name == this.JobName);

            if(isExistingJob)
            {
                this.Schedule.RescheduleJob(this.Trigger.Key, this.Trigger);
            }
            else
            {
                this.Schedule.ScheduleJob(this.Job, this.Trigger);
            }          
        }        
    }
}