namespace Fanex.NAP.CMS.VirtualSetting.Console
{
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;

    public class Service : BaseService
    {
        public override BaseService ExecuteForSoccer()
        {
            VirtualSettingSoccerService service = new VirtualSettingSoccerService(AppSetting.SiteIds, AppSetting.SuperIds, DateTime.Now);

            service.AutoAdjustVirtualSettingsForPunters();

            PuntersSoccer = service.Punters;

            return this;
        }

        public override BaseService ExecuteForSportbooks()
        {
            VirtualSettingSportsbookService service = new VirtualSettingSportsbookService(AppSetting.SiteIds, AppSetting.SuperIds, DateTime.Now);

            service.AutoAdjustVirtualSettingsForPunters();

            PuntersSportbooks = service.Punters;

            return this;
        }

        public override BaseService SendMail()
        {
            return this;
        }
    }
}