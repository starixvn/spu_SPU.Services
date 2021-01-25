namespace Fanex.NAP.SPU.Gateway.Controllers
{
    using Fanex.NAP.SPU.Gateway.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System; 

    [ApiController]
    [Route("[controller]")]
    public class SchedulerController : ControllerBase
    {
        private const string jobTemplate = @"App_Data\Job.json";
        private readonly ILogger<SchedulerController> _logger;

        public SchedulerController(ILogger<SchedulerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public SchedulerModel Get()
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + jobTemplate;

            var jobMappings = System.IO.File.ReadAllText(filePath);

            var schedule = JsonConvert.DeserializeObject<SchedulerModel>(jobMappings);

            return schedule;
        }

    }
}
