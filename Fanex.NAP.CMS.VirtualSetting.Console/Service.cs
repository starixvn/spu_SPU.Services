namespace Fanex.NAP.CMS.VirtualSetting.Console
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Linq;

    public class Service : BaseService
    {
        public bool IsStopService()
        {
            return AppSetting.IsStopService;
        }

        public bool IsExecutedDate()
        {
            var currentDate = DateTime.Today;
            var specialDay = AppSetting.SpeciaExecuteDate;
            var isSpecialDay = currentDate.Year == specialDay.Year && currentDate.Month == specialDay.Month && currentDate.Day == specialDay.Day;

            return currentDate.Day == 1 || currentDate.Day == 16 || isSpecialDay;
        }

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

        public override BaseService CountAffectedMembers()
        {
            if (PuntersSoccer != null && PuntersSportbooks != null)
            {
                TotalAffectedMembers = PuntersSoccer.Union(PuntersSportbooks, new PunterEqualityComparer()).Count();
            }
            else if (PuntersSoccer != null)
            {
                TotalAffectedMembers = PuntersSoccer.Count();
            }
            else if (PuntersSportbooks != null)
            {
                TotalAffectedMembers = PuntersSportbooks.Count();
            }
            else
            {
                TotalAffectedMembers = 0;
            }

            return this;
        }

        public override BaseService SendMail()
        {
            return base.SendMail();
        }
    }
}