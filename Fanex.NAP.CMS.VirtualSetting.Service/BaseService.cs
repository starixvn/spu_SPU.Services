namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Data;
    using Fanex.NAP.SPU.Core;

    public class BaseService
    {
        protected int PuntersUpdatedSoccer { get; set; }

        protected int PuntersUpdatedSportbooks { get; set; }

        protected int TotalAffectedMembers { get; set; }

        public BaseService()
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .UseDefaultConnectionStringProvider()
                  .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                  .Run();
        }

        public virtual BaseService ExecuteForSoccer() { return this; }

        public virtual BaseService ExecuteForSportbooks() { return this; }

        public virtual BaseService CountAffectedMembers() { return this; }

        public virtual BaseService SendMailExternal(int mailingListId) { return this; }

        public virtual BaseService SendMailInternal(int mailingListId) { return this; }
    }
}