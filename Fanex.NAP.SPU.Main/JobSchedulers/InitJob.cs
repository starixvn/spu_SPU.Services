using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    public static class InitJob
    {
        public static void AddSingletonJob(this IServiceCollection services)
        {
            var jobSchedules = new List<JobSchedule>() {
                new JobSchedule(
                   jobType: typeof(HelloJob),
                   cronExpression: "0/5 * * * * ?",
                   apiUrl: "")
            };
                       

            services.AddSingleton(new JobSchedules(jobSchedules)); //every 5 seconds
        }

    }
}
