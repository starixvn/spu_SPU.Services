namespace Fanex.NAP.SPU.Demo.JobService.Controllers
{
    using Fanex.NAP.SPU.Demo.JobService.Service;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ISentryClient sentryClient;

        public JobController(IMediator mediator, ISentryClient sentryClient)
        {
            this.mediator = mediator;
            this.sentryClient = sentryClient;
        }

        [HttpPost("execute")]
        public async Task<bool> Execute(string name)
        {
            TestData();

            var result = await mediator.Send(new JobRequest(name));

            this.sentryClient.CaptureMessage($"{name} is running at {DateTime.Now}");

            return result;
        }

        private void TestData()
        {
            IService service = new Service();

            var data = service.GetPreviousTimingExecution(3);
        }
    }
}
