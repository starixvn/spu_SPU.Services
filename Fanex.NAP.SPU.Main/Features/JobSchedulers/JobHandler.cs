using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers
{
    public class JobHandler : IRequestHandler<JobRequest, bool>
    {
        private readonly IJobService jobService;

        public JobHandler(IJobService jobService)
        {
            this.jobService = jobService;
        }

        public async Task<bool> Handle(JobRequest request, CancellationToken cancellationToken)
        {
            return await jobService.Execute(request.Name);
        }
    }
}
