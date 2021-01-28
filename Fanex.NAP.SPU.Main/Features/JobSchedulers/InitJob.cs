namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;    
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers;
    using Refit;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.Middleware;    

    public static class InitJob
    {
        public static void AddSingletonJob(this IServiceCollection services, AppSettings appSettings)
        {
            var schedule = GetSchedule(appSettings.GatewayURL).Result;

            if (schedule != null && schedule.GroupJobs.Any())
            {
                var jobSchedules = GetJobSchedules(schedule.GroupJobs);

                services.AddSingleton(jobSchedules);

                services
               .AddHealthChecksUI(setup =>
               {
                   setup.AddHealthCheckEndpoint("App", "http://localhost:53278/healthz");

                   foreach (var job in jobSchedules.JobScheduleList)
                   {
                       if (!string.IsNullOrWhiteSpace(job.HealthCheckUrl))
                       {
                           setup.AddHealthCheckEndpoint(job.JobName, job.HealthCheckUrl);
                       }
                   }                                      
               })
               .AddInMemoryStorage();
               //.AddMySqlStorage(appSettings.HealthCheckStorage);
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
