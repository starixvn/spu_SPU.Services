namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Data.MySql;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using System;

    public class BaseService
    {
        public int FraudCaseId { get; set; }

        public BaseService(FraudCaseId fraudCaseId)
        {
            DbSettingProviderManager
                 .StartNewSession()
                 .WithSqlServer()
                 .WithMySql()
                 .UseDefaultDbSettingProvider(AppSetting.AppData)
                 .UseDefaultConnectionStringProvider()
                 .Run();

            FraudCaseId = Convert.ToInt32(fraudCaseId);
        }

        public virtual BaseService Precondition() { return this; }

        public virtual BaseService Rule() { return this; }

        public virtual BaseService CleanResult() { return this; }

        private bool GetFraudCaseActive()
        {
            var fraudCaseService = new FraudCaseService();

            var fraudCase = fraudCaseService.GetByFraudCaseId(FraudCaseId, AppSetting.Environment);

            if (fraudCase == null)
            {               
                return false;
            }
            
            return fraudCase.IsActive;
        }

        public bool CheckFraudCaseIsActive()
        {
           return GetFraudCaseActive();
        }
    }
}
