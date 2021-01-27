using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Demo.JobService.Controllers
{
    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly IMediator mediator;

        public JobController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("execute")]
        public async Task<bool> Execute(string name)
        {
            var result = await mediator.Send(new JobRequest(name));

            return result;
        }
    }
}
