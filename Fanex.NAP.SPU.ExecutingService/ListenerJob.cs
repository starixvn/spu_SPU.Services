namespace Fanex.NAP.SPU.ExecutingService
{
    using System.Threading;
    using System.Threading.Tasks;
    using Quartz;
    using Fanex.Logging;
    using System;
    using Fanex.NAP.SPU.Service;
    using Fanex.NAP.FS.Core;

    public class ListenerJob : IJobListener
    {
        private const string TriesKey = "TriesKey";
        private const int MaxRetries = 10;
        private readonly string _jobName;
        private readonly LetstalkService _letstalkService;
        public static readonly string TriggerGroup = "failTriggerGroup";

        public string Name => _jobName;

        public ListenerJob(string jobName)
        {
            _jobName = jobName;
            _letstalkService = new LetstalkService();
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            LogService.Error($"{context.JobDetail.Key} - JobExecutionVetoed - Job is Vetoed.");

            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!context.JobDetail.JobDataMap.Contains(TriesKey))
            {
                context.JobDetail.JobDataMap.Put(TriesKey, 0);
            }

            var numberTries = context.JobDetail.JobDataMap.GetIntValue(TriesKey);

            context.JobDetail.JobDataMap.Put(TriesKey, ++numberTries);

            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobException == null)
            {
                return;
            }

            var numTries = context.JobDetail.JobDataMap.GetIntValue(TriesKey);

            if (numTries > MaxRetries)
            {
                LogService.Error(string.Format("Job with ID and type: {0}, {1} has run {2} times and has failed each time.",
                context.JobDetail.Key, context.JobDetail.JobType, MaxRetries));

                return;
            }

            var errorMessage = string.Format("Job with ID and type: {0}, {1} has thrown the exception: {2}. Running again in {3} seconds.",
            context.JobDetail.Key, context.JobDetail.JobType, jobException.Message, 15 * numTries);

            LogService.Error(errorMessage);

            if (AppSetting.Environment == 3)
            {               
                _letstalkService.Error(errorMessage);                
            }

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(Guid.NewGuid().ToString(), TriggerGroup)
                .StartAt(DateTime.Now.AddSeconds(15 * numTries))
                .Build();

            await context.Scheduler.RescheduleJob(context.Trigger.Key, trigger);
        }
    }
}