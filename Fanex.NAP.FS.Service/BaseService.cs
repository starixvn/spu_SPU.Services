namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.Data.ConnectionStringProvider20;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using RestSharp;
    using System;

    public class BaseService
    {
        public int FraudCaseId { get; set; }

        protected IUMControlAdapter maintenanceService;

        public BaseService(FraudCaseId fraudCaseId)
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .Use(new ConnectionStringProvider20())
                  .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                  .Run();
        
            FraudCaseId = Convert.ToInt32(fraudCaseId);

            maintenanceService = new UMControlAdapter();
        }

        public virtual BaseService Precondition() { return this; }

        public virtual BaseService Rule() { return this; }

        public virtual BaseService CleanResult() { return this; }        

        public bool CheckUM()
        {
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }
    }
}
