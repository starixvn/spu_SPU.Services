namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Fanex.NAP.SPU.Main.Features.Healthchecks.Services;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Fanex.NAP.SPU.Middleware;
    using Microsoft.Extensions.DependencyInjection;
    using Refit;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class JobInit
    {
        public static void AddSingletonJob(this IServiceCollection services, AppSettings appSettings)
        {
            var schedule = GetSchedule(appSettings.GatewayURL).Result;

            if (schedule != null && schedule.GroupJobs.Any())
            {
                var jobSchedules = GetJobSchedules(schedule.GroupJobs);

                services.AddSingleton(jobSchedules);

                services.AddSingletonHealthCheck(appSettings, jobSchedules);
            }
        }

        private async static Task<SchedulerModel> GetSchedule(string url)
        {
            var response = RestService.For<IJobServiceApi>(url);

            return await response.GetScheduler();
        }

        public static JobSchedules GetJobSchedules(IEnumerable<GroupJobModel> groupJobList)
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
