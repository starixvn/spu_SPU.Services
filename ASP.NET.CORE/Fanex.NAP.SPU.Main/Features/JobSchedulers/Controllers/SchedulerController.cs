namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Controllers
{
    using Fanex.NAP.SPU.Main.JobSchedulers;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NAP.SPU.Middleware;
    using Refit;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;

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
        public IActionResult CheckUpdateJobs()
        {
            var schedulerModel = GetSchedules(_appSettings.GatewayURL).Result;            

            foreach (var group in schedulerModel.GroupJobs)
            {
                CheckUpdateJob(group.Jobs, _quartzHostedService.CurrentJobSchedules.JobScheduleList);
            }

            return View(schedulerModel.GroupJobs);
        }

        private void CheckUpdateJob(IEnumerable<JobModel> jobs, List<JobScheduleViewModel> currentJobSchedules)
        {
            foreach (var job in jobs)
            {
                var isNoUpdateJob = currentJobSchedules.Any(x => x.JobName == job.JobName
                        && x.JobType.FullName == job.JobType
                        && x.APIUrl == job.APIUrl
                        && x.IsActive == job.IsActive);

                job.UpdateStatus = isNoUpdateJob ? UpdateStatus.NoUpdate : UpdateStatus.Update;

                if (job.UpdateStatus == UpdateStatus.NoUpdate)
                {
                    CheckUpdateTrigger(job, job.Triggers, currentJobSchedules);
                }
            }
        }

        private void CheckUpdateTrigger(JobModel job, IEnumerable<TriggerModel> triggers, List<JobScheduleViewModel> currentJobSchedules)
        {
            foreach (var trigger in triggers)
            {
                var isTriggerNoUpdate = currentJobSchedules.Any(x => x.TriggerName == trigger.TriggerName
                    && x.TriggerType == trigger.TriggerType
                    && x.RepeatForever == trigger.RepeatForever
                    && x.CronExpression == trigger.CronExpression);

                trigger.UpdateStatus = isTriggerNoUpdate ? UpdateStatus.NoUpdate : UpdateStatus.Update;

                if (trigger.UpdateStatus == UpdateStatus.Update)
                {                    
                    job.UpdateStatus = UpdateStatus.Update;
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> SaveUpdateJobs()
        {
            var schedulerModel = GetSchedules(_appSettings.GatewayURL).Result;

            await _quartzHostedService.UpdateSchedule(schedulerModel);

            var viewModel = new SaveUpdateJobViewModel { Message = "Save Successfully" };

            return View(viewModel);
        }

        private async Task<SchedulerModel> GetSchedules(string url)
        {
            var response = RestService.For<IJobServiceApi>(url);

            return await response.GetScheduler();
        }

    }
}
