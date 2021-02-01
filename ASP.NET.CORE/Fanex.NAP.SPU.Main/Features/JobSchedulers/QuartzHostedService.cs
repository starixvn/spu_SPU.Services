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

        public JobSchedules CurrentJobSchedules { get { return _jobSchedules; } }

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

        public UpdateJobViewModel CheckUpdateSchedule(SchedulerModel newSchedule)
        {
            var newJobSchedules = InitJob.GetJobSchedules(newSchedule.GroupJobs);

            var updateList = CheckUpdate(_jobSchedules, newJobSchedules);

            var newList = CheckNew(_jobSchedules, newJobSchedules);

            var deleteList = CheckDelete(_jobSchedules, newJobSchedules);

            return new UpdateJobViewModel(newList);
        }

        public async Task<UpdateJobViewModel> UpdateSchedule(SchedulerModel newSchedule)
        {            
            var scheduler = await _schedulerFactory.GetScheduler();
            
            var newJobSchedules = JobInit.GetJobSchedules(newSchedule.GroupJobs);

            var updateList = CheckUpdate(_jobSchedules, newJobSchedules);

            var newList = CheckNew(_jobSchedules, newJobSchedules);

            var deleteList = CheckDelete(_jobSchedules, newJobSchedules);

            DeleteJobTrigger(scheduler, deleteList);

            UpdateJobTrigger(scheduler, updateList);

            NewJobTrigger(scheduler, newList);

            _jobSchedules.UpdateJobScheduleList(newJobSchedules.JobScheduleList);

            return new UpdateJobViewModel(newList);
        }

        public IEnumerable<JobScheduleViewModel> CheckUpdate(JobSchedules oldJobSchedules, JobSchedules newJobSchedules)
        {
            return oldJobSchedules.JobScheduleList.Join(newJobSchedules.JobScheduleList,
                x1 => x1.JobName,
                x2 => x2.JobName,
                (x1, x2) => x1);
        }

        public IEnumerable<JobScheduleViewModel> CheckNew(JobSchedules oldJobSchedules, JobSchedules newJobSchedules)
        {
            var oldJobNameList = oldJobSchedules.JobScheduleList.Select(x => x.JobName);

            return newJobSchedules.JobScheduleList.Where(x => !oldJobNameList.Contains(x.JobName));
        }

        public IEnumerable<JobScheduleViewModel> CheckDelete(JobSchedules oldJobSchedules, JobSchedules newJobSchedules)
        {
            var newJobNameList = newJobSchedules.JobScheduleList.Select(x => x.JobName);

            return oldJobSchedules.JobScheduleList.Where(x => !newJobNameList.Contains(x.JobName));
        }

        private bool IsNewTrigger(JobScheduleViewModel newSchedule, JobSchedules oldJobSchedules)
        {
            var existed = oldJobSchedules.JobScheduleList.Any(x => x.TriggerName == newSchedule.TriggerName);

            return !existed;
        }

        private async void DeleteJobTrigger(IScheduler scheduler, IEnumerable<JobScheduleViewModel> jobScheduleList)
        {
            foreach (var jobSchedule in jobScheduleList)
            {
                await scheduler.DeleteJob(new JobKey(jobSchedule.JobName));
            }
        }

        private async void UpdateJobTrigger(IScheduler scheduler, IEnumerable<JobScheduleViewModel> jobScheduleList)
        {
            foreach (var jobSchedule in jobScheduleList)
            {
                var trigger = CreateTrigger(jobSchedule);

                var isNewTrigger = IsNewTrigger(jobSchedule, _jobSchedules);

                if (isNewTrigger)                
                    await scheduler.ScheduleJob(trigger);                
                else                
                    await scheduler.RescheduleJob(new TriggerKey(jobSchedule.TriggerName), trigger);

                if (!jobSchedule.IsActive)                
                    await scheduler.PauseTrigger(new TriggerKey(jobSchedule.TriggerName));                
            }
        }

        private async void NewJobTrigger(IScheduler scheduler, IEnumerable<JobScheduleViewModel> jobScheduleList)
        {
            foreach (var jobSchedule in jobScheduleList)
            {
                if (!jobSchedule.Validate())
                    continue;

                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await scheduler.ScheduleJob(job, trigger);

                if (!jobSchedule.IsActive)
                    await scheduler.PauseTrigger(new TriggerKey(jobSchedule.TriggerName));
            }
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
