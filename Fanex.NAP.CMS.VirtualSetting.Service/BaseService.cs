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

        protected int TotalAffectedForSpecicalSport { get; set; }

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

        public virtual BaseService UpdateVRForSoccer() { return this; }

        public virtual BaseService UpdateVRForBasketball() { return this; }

        public virtual BaseService UpdateVRForSpecialSportsbook() { return this; }

        public virtual BaseService UpdateForSportbooks() { return this; }

        public virtual BaseService UpdateBetLimit() { return this; }

        public virtual BaseService UpdateVRAgent() { return this; }

        public virtual BaseService CountAffectedMembers() { return this; }

        public virtual BaseService SendMailExternal(int mailingListId) { return this; }

        public virtual BaseService NotifyMessage() { return this; }

        public virtual BaseService SendMailInternal(int mailingListId) { return this; }       
        


    }
}