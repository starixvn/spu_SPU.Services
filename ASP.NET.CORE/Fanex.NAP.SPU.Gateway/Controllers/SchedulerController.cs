namespace Fanex.NAP.SPU.Gateway.Controllers
{
    using Fanex.NAP.SPU.Gateway.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class SchedulerController : ControllerBase
    {
        private readonly ILogger<SchedulerController> _logger;
        private readonly IConfiguration _configuration;

        public SchedulerController(IConfiguration configuration, ILogger<SchedulerController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public SchedulerModel Get()
        {
            var schedule = new SchedulerModel();

            _configuration.GetSection("SchedulerModel").Bind(schedule);

            return schedule;
        }

    }
}
