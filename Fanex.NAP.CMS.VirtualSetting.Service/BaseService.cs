namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Data;
    using Fanex.Data.ConnectionStringProvider20;
    using Fanex.NAP.SPU.Core;

    public class BaseService
    {
        public BaseService()
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .Use(new ConnectionStringProvider20())
                  .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                  .Run();            
        }

        public virtual BaseService ExecuteForSoccer() { return this; }

        public virtual BaseService ExecuteForSportbooks() { return this; }

        public virtual BaseService SendMail() { return this; }

    }
}