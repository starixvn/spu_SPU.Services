using Fanex.NAP.SPU.Main.Features.JobSchedulers.Services;
using Quartz;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    [DisallowConcurrentExecutionAttribute]
    public class JobExecute : IJob
    {
        
        private readonly ISentryClient sentryClient;
        private readonly IJobService jobService;
        public JobExecute(ISentryClient sentryClient, IJobService jobService)
        {            
            this.sentryClient = sentryClient;
            this.jobService = jobService;
        }

        public async Task Execute(IJobExecutionContext context) 
        {
            var jobName = context.JobDetail.Key.Name;
            var apiUrl = context.JobDetail.JobDataMap.Get("apiUrl").ToString();

            var result = await jobService.Execute(jobName, apiUrl);

            if (!result)
            {
                var errorMessage = $"{jobName} is failed at {DateTime.Now}.";               
                var jobExecutionException = new JobExecutionException(errorMessage);

                sentryClient.CaptureMessage(errorMessage, Sentry.Protocol.SentryLevel.Error);

                throw jobExecutionException;
            }
        }
    }
}
