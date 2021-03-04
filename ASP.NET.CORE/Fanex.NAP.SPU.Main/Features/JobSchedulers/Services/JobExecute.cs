using Fanex.NAP.SPU.Main.Features.JobSchedulers.Services;
using MediatR;
using Quartz;
using Sentry;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    [DisallowConcurrentExecutionAttribute]
    public class JobExecute : IJob
    {
        private readonly IMediator mediator;
        private readonly ISentryClient sentryClient;

        public JobExecute(IMediator mediator, ISentryClient sentryClient)
        {
            this.mediator = mediator;
            this.sentryClient = sentryClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            var apiUrl = context.JobDetail.JobDataMap.Get("apiUrl").ToString();
             await mediator.Send(new JobRequest(jobName, apiUrl));
        }
    }
}
