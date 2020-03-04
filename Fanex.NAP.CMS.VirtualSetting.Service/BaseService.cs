namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Service;

    public class BaseService
    {
        protected int PuntersUpdatedSoccer { get; set; }

        protected int PuntersUpdatedSportbooks { get; set; }

        protected int TotalAffectedMembers { get; set; }

        protected LetstalkService _letstalkService;

        public BaseService()
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer()
                .UseDefaultDbSettingProvider(AppSettings.AppDataSetting)
                .UseDefaultConnectionStringProvider()
                .Run();

            _letstalkService = new LetstalkService();
        }

        public virtual BaseService ExecuteForSoccer() { return this; }

        public virtual BaseService ExecuteForSportbooks() { return this; }

        public virtual BaseService CountAffectedMembers() { return this; }

        public virtual BaseService SendMailExternal(int mailingListId) { return this; }

        public virtual BaseService SendMailInternal(int mailingListId) { return this; }
    }
}