using Fanex.NAP.SPU.Main.Features.JobSchedulers.Services;
using Quartz;
using Sentry;
using System;
using System.Threading;
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
            var dataMap = context.JobDetail.JobDataMap;
            var jobName = context.JobDetail.Key.Name;
            var apiUrl = dataMap.GetString("apiUrl");
            var count = dataMap.GetInt("count");

            if(count >= 5)
            {
                var e = new JobExecutionException("Retries exceeded");
                //unschedule it so that it doesn't run again
                e.UnscheduleAllTriggers = true;

                sentryClient.CaptureMessage($"{jobName} is STOPED. Cause of RETRIED EXECEEDED.", Sentry.Protocol.SentryLevel.Error);

                throw e;
            }

            try
            {
                var result = await jobService.Execute(jobName, apiUrl);

                if (!result)
                {
                    count++;

                    dataMap.PutAsString("count", count);
                    var errorMessage = $"{jobName} is failed at {DateTime.Now}.";                   

                    throw JobExecuteFailed(errorMessage);
                }
                else
                {
                    dataMap.PutAsString("count", 0);
                }
            }
            catch (Exception ex)
            {
                count++;

                dataMap.PutAsString("count", count);

                throw JobExecuteFailed(ex.Message);
            }
        }

        private JobExecutionException JobExecuteFailed(string errorMessage)
        {            
            var jobExecutionException = new JobExecutionException(errorMessage);

            sentryClient.CaptureMessage(errorMessage, Sentry.Protocol.SentryLevel.Error);

            Thread.Sleep(20000); //sleep for 20 secs

            jobExecutionException.RefireImmediately = true;

            return jobExecutionException;
        }
    }
}
