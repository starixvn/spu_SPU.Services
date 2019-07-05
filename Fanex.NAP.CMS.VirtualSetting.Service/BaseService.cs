namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Data;
    using Fanex.Data.ConnectionStringProvider20;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System.Collections.Generic;

    public class BaseService
    {
        protected IEnumerable<Punter> PuntersSoccer { get; set; }

        protected IEnumerable<Punter> PuntersSportbooks { get; set; }

        protected int TotalAffectedMembers { get; set; }

        public BaseService()
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .UseDefaultConnectionStringProvider()
                  //.Use(new ConnectionStringProvider20())
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