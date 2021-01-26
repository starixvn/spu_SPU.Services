namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Controllers
{
    using Fanex.NAP.SPU.Main.JobSchedulers;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;    
    using Fanex.NAP.SPU.Main.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;

    public class SchedulerController : Controller
    {
        private readonly ILogger<SchedulerController> _logger;        
        private readonly JobSchedules _jobSchedules;
        private readonly QuartzHostedService _quartzHostedService;

        public SchedulerController(ILogger<SchedulerController> logger
            , JobSchedules jobSchedules
            , QuartzHostedService quartzHostedService)
        {
            _logger = logger;        
            _jobSchedules = jobSchedules;
            _quartzHostedService = quartzHostedService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateJobs()
        {
            //var apiUrl = "http://localhost:60823/Scheduler";

            //var schedulerModel = ApiHelper.Get<SchedulerModel>(apiUrl).Result;

            //_schedulerProvider.UpdateJobs(schedulerModel);

            //_quartzHostedService.AddJob();

            return View();
        }
    }
}
