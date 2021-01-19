namespace Fanex.NAP.SPU.Gateway.Controllers
{
    using Fanex.NAP.SPU.Gateway.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;

        public JobController(ILogger<JobController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> GetJobs()
        {
            var jobs = Serializer.Deserialize<string>("");

            return new List<string>();
        }

    }
}
