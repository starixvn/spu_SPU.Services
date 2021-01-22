namespace Fanex.NAP.SPU.Main.Controllers
{
    using Fanex.NAP.SPU.Main.JobSchedulers;
    using Fanex.NAP.SPU.Main.Models;
    using Fanex.NAP.SPU.Main.Provider;
    using Fanex.NAP.SPU.Main.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;

    public class SchedulerController : Controller
    {
        private readonly ILogger<SchedulerController> _logger;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly JobSchedules _jobSchedules;

        public SchedulerController(ILogger<SchedulerController> logger, ISchedulerProvider schedulerProvider, JobSchedules jobSchedules)
        {
            _logger = logger;
            _schedulerProvider = schedulerProvider;
            _jobSchedules = jobSchedules;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateJobs()
        {            
            try
            {
                var apiUrl = "http://localhost:60823/Scheduler";

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
