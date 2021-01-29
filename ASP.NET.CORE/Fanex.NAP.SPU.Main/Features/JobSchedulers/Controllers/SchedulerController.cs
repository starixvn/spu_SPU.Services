namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Controllers
{
    using Fanex.NAP.SPU.Main.JobSchedulers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;    
    using Fanex.NAP.SPU.Middleware;
    using Refit;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using System.Threading.Tasks;

    public class SchedulerController : Controller
    {
        private readonly JobSchedules _jobSchedules;
        private readonly QuartzHostedService _quartzHostedService;
        private readonly IAppSettings _appSettings;

        public SchedulerController(JobSchedules jobSchedules
            , QuartzHostedService quartzHostedService
            , IAppSettings appSettings)
        {
            _jobSchedules = jobSchedules;
            _quartzHostedService = quartzHostedService;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateJobs()
        {
            var response = GetSchedule(_appSettings.GatewayURL).Result;

           var viewModel =  await _quartzHostedService.UpdateSchedule(response);          

            return View(viewModel);
        }

        private async Task<SchedulerModel> GetSchedule(string url)
        {
            var response = RestService.For<IJobServiceApi>(url);

            return await response.GetScheduler();
        }
    }
}
