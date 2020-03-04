namespace Fanex.NAP.SPU.ExecutingService
{
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;

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
