namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.NAP.SPU.Model;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Quartz;

    public class ExecuteJobBase
    {
        protected IUMControlAdapter maintenanceService;

        public ExecuteJobBase()
        {
            maintenanceService = new UMControlAdapter();
        }      

        public bool CheckUM()
        {
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }
    }
}
