namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Models;
    using Fanex.NAP.SPU.Main.Utilities;
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

        private static IEnumerable<JobModel> GetJobs()
        {
            var jobList = Enumerable.Empty<JobModel>();

            try
            {
                var apiUrl = "http://localhost:60823/Scheduler";

                var scheduler = ApiHelper.Get<SchedulerModel>(apiUrl).Result;

                if (scheduler != null && scheduler.Jobs.Any())
                {
                    jobList = scheduler.Jobs;
                }

                return jobList;
            }
            catch (Exception ex)
            {
                return jobList;
            }
        }

        private static JobSchedules GetJobSchedules(IEnumerable<JobModel> jobList)
        {
            var jobScheduleList = new List<JobSchedule>();

            foreach (var job in jobList)
            {
                foreach (var trigger in job.Triggers)
                {
                    jobScheduleList.Add(new JobSchedule(job, trigger));                       
                }
            }

            return new JobSchedules(jobScheduleList);
        }

    }
}
