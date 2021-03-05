namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using ONEbook.UM.Services;
    using Quartz;
    using System;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class QuartzJobRunner : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMaintenanceService _maintenanceService;

        public QuartzJobRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _maintenanceService = new MaintenanceService();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var isUM = await CheckUM();

            if (!isUM)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var job = scope.ServiceProvider.GetRequiredService(context.JobDetail.JobType) as IJob;
                    
                    await job.Execute(context);
                }
            }
        }

        private async Task<bool> CheckUM()
        {            
            var underMaintenanceInfo = await _maintenanceService.GetUnderMaintenanceTime();

            return underMaintenanceInfo.IsUnderMaintenanceTime;
        }

    }
}
