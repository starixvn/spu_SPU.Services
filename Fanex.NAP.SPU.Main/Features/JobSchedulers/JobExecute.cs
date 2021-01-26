using Fanex.NAP.SPU.Main.Features.JobSchedulers;
using MediatR;
using Quartz;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    public class JobExecute : IJob
    {
        //private readonly IMediator mediator;

        //public JobExecute(IMediator mediator)
        //{
        //    this.mediator = mediator;
        //}

        //public Task Execute(IJobExecutionContext context)
        //{
        //    var jobName = context.JobDetail.Key.Name;

        //    return mediator.Send(new JobRequest(jobName));
        //}
        public Task Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
