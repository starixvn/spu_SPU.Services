namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Data;
    using Fanex.Data.ConnectionStringProvider20;
    using Fanex.NAP.SPU.Core;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;

    public class BaseService
    {
        protected IUMControlAdapter maintenanceService;

        public BaseService()
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .Use(new ConnectionStringProvider20())
                  .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                  .Run();

            maintenanceService = new UMControlAdapter();
        }

        public bool CheckUM()
        {
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }

    }
}