namespace Fanex.NAP.SPU.Main.JobSchedulers
{    
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Spi; 
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

        public async Task AddJob()
        {
            //var scheduler = await _schedulerFactory.GetScheduler();

            //foreach (var jobSchedule in _jobSchedules.JobScheduleList)
            //{
            //    jobSchedule.JobName = jobSchedule.JobName + "_New_" + System.DateTime.Now.ToLongTimeString();
            //    var job = CreateJob(jobSchedule);
                
            //    var trigger = CreateTrigger(jobSchedule);

            //    await scheduler.ScheduleJob(job, trigger);
            //}

            await Task.CompletedTask;
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
