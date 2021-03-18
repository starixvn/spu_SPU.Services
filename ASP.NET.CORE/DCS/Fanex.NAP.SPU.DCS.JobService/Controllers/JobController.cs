using Fanex.NAP.SPU.DCS.JobService.Service;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IJobService jobService;

        public JobController(ISentryClient sentryClient, IJobService jobService)
        {
            this.sentryClient = sentryClient;
            this.jobService = jobService;
        }

        [HttpPost("execute")]
        public async Task<bool> Execute(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.jobService.Execute(jobName);

        }
    }
}
