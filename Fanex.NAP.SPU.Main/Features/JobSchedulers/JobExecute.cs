namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers;
    using MediatR;
    using Quartz;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class JobExecute : IJob
    {
        private readonly IMediator mediator;

        public JobExecute(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;

            return mediator.Send(new JobRequest(jobName));
        }
    }
}
