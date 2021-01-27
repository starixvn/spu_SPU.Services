namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Spi;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;    

    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly JobSchedules _jobSchedules;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            JobSchedules jobSchedules,            
            IJobFactory jobFactory)
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;            
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules.JobScheduleList)
            {
                if(!jobSchedule.Validate())
                    continue;

                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.Shutdown(cancellationToken);
        }

        public async Task UpdateSchedule(SchedulerModel newSchedule)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobSchedules = InitJob.GetJobSchedules(newSchedule.GroupJobs);

            foreach (var jobSchedule in _jobSchedules.JobScheduleList)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                var isNewJob = IsNewJob(jobSchedule, jobSchedules);
                var isNewTrigger = IsNewTrigger(jobSchedule, jobSchedules);

                if (isNewJob)
                    await scheduler.ScheduleJob(job, trigger);
                else if(isNewTrigger)
                    await scheduler.ScheduleJob(trigger);
                else
                    await scheduler.RescheduleJob(new TriggerKey(jobSchedule.TriggerName), trigger);
            }

            await Task.CompletedTask;
        }
      
        private bool IsNewJob(JobScheduleViewModel schedule, JobSchedules jobSchedules)
        {
            var existed = jobSchedules.JobScheduleList.Any(x => x.JobName == schedule.JobName);

            return !existed;
        }

        private bool IsNewTrigger(JobScheduleViewModel schedule, JobSchedules jobSchedules)
        {
            var existed = jobSchedules.JobScheduleList
                                      .Any(x => x.TriggerName == schedule.TriggerName
                                          && x.CronExpression == schedule.CronExpression);

            return !existed;
        }

        private static IJobDetail CreateJob(JobScheduleViewModel schedule)
        {
            return JobBuilder
                .Create(schedule.JobType)
                .WithIdentity(schedule.JobName, schedule.Group)
                .WithDescription(schedule.JobName)
                .UsingJobData("apiUrl", schedule.APIUrl)
                .Build();
        }

        private static ITrigger CreateTrigger(JobScheduleViewModel schedule)
        {
            var builder = TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.TriggerName}")
                .WithDescription(schedule.TriggerName);

            schedule.Cron.Apply(builder, schedule);

            return builder.Build();
        }
       
    }
}
