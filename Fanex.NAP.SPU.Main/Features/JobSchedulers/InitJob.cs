namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.Utilities;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class InitJob
    {
        public static void AddSingletonJob(this IServiceCollection services)
        {
            var jobs = GetJobs();

            var jobSchedules = GetJobSchedules(jobs);

            services.AddSingleton(jobSchedules);
        }

        private static IEnumerable<GroupJobModel> GetJobs()
        {
            var groupJob = new List<GroupJobModel>();

            var apiUrl = "http://localhost:60823/Scheduler";

            var scheduler = ApiHelper.Get<SchedulerModel>(apiUrl).Result;

            if (scheduler != null && scheduler.GroupJobs.Any())
            {
                groupJob.AddRange(scheduler.GroupJobs);
            }

            return groupJob;
        }

        private static JobSchedules GetJobSchedules(IEnumerable<GroupJobModel> groupJobList)
        {
            var jobScheduleList = new List<JobScheduleViewModel>();

            foreach (var group in groupJobList)
            {
                foreach (var job in group.Jobs)
                {
                    foreach (var trigger in job.Triggers)
                    {
                        jobScheduleList.Add(new JobScheduleViewModel(group.GroupName, job, trigger));
                    }
                }                
            }

            return new JobSchedules(jobScheduleList);
        }

    }
}
