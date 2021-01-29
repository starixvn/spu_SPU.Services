using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Demo.JobService2.Controllers
{
    public class JobHandler : IRequestHandler<JobRequest, bool>
    {
        private readonly IJobService jobService;
        public JobHandler(IJobService jobService)
        {
            this.jobService = jobService;
        }

        public Task<bool> Handle(JobRequest request, CancellationToken cancellationToken) => this.jobService.Update(request.Name);
    }
}
