namespace Fanex.NAP.SPU.Main.Controllers
{
    using Fanex.NAP.SPU.Main.JobSchedulers;
    using Fanex.NAP.SPU.Main.Models;
    using Fanex.NAP.SPU.Main.Provider;
    using Fanex.NAP.SPU.Main.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SchedulerController : Controller
    {
        private readonly ILogger<SchedulerController> _logger;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly JobScheduler jobScheduler;

        public SchedulerController(ILogger<SchedulerController> logger, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _schedulerProvider = schedulerProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateJobs()
        {
            var apiEndpoint = "Scheduler";
            var apiUrl = "http://localhost:60823/" + apiEndpoint;

            try
            {
                var schedulerModel = ApiHelper.Get<SchedulerModel>(apiUrl).Result;

                _schedulerProvider.UpdateJobs(schedulerModel);
            }
            catch (Exception ex)
            {
                
            }

            return View();
        }
    }
}
