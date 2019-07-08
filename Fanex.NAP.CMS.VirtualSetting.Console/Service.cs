namespace Fanex.NAP.CMS.VirtualSetting.Console
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;

    public class Service : BaseService
    {
        private DateTime _startTime;

        private DateTime _stopTime;

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

        public Service StartTime()
        {
            _startTime = DateTime.Now;

            return this;
        }

        public override BaseService ExecuteForSoccer()
        {
            VirtualSettingSoccerService service = new VirtualSettingSoccerService(AppSetting.SiteIds, AppSetting.SuperIds, DateTime.Now);

            service.AutoAdjustVirtualSettingsForPunters();

            PuntersUpdatedSoccer = service.PuntersUpdated;

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService ExecuteForSportbooks()
        {
            VirtualSettingSportsbookService service = new VirtualSettingSportsbookService(AppSetting.SiteIds, AppSetting.SuperIds, DateTime.Now);

            service.AutoAdjustVirtualSettingsForPunters();

            PuntersUpdatedSportbooks = service.PuntersUpdated;

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService CountAffectedMembers()
        {
            TotalAffectedMembers = PuntersUpdatedSoccer + PuntersUpdatedSportbooks;

            return this;
        }

        public override BaseService SendMailExternal(int mailingListId)
        {
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"MailTemplate\MailInformTemplate.cshtml";

            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, _startTime.ToString("MM/dd/yyyy hh:mm:ss"), _stopTime.ToString("MM/dd/yyyy hh:mm:ss"), TotalAffectedMembers);

            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Adjustment finished" + " (" + AppSetting.Environment + ")",
                Title = "VR Auto Mail Service",
                Body = body,
                MailingListId = mailingListId
            };

            //MailClientHandler.SendFunctionMail(newMail);

            LogService.Error($"VirtualSetting service - SendMailExternal successful");

            return this;
        }

        public override BaseService SendMailInternal(int mailingListId)
        {
            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Mail Service - Still alive" + " (" + AppSetting.Environment + ")",
                Title = "VR Auto Mail Service - Still alive",
                Body = "Is NOT the execution date for Updating Virtual Settings",
                MailingListId = mailingListId
            };

            //MailClientHandler.SendFunctionMail(newMail);

            LogService.Error($"VirtualSetting service - SendMailInternal successful");

            return this;
        }
    }
}