using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.JobSchedulers
{
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

        public async Task AddJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            foreach (var jobSchedule in _jobSchedules.JobScheduleList)
            {
                jobSchedule.JobName = jobSchedule.JobName + "_New_" + System.DateTime.Now.ToLongTimeString();
                var job = CreateJob(jobSchedule);
                
                var trigger = CreateTrigger(jobSchedule);

                await scheduler.ScheduleJob(job, trigger);
            }

            await Task.CompletedTask;
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {            
            return JobBuilder
                .Create(schedule.JobType)
                .WithIdentity(schedule.JobName)
                .WithDescription(schedule.JobName)
                .Build();
        }
    }
}
