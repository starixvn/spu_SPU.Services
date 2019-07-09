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

        public bool IsExecutedDate()
        {
            var currentDate = DateTime.Today;

            return currentDate.Day == 1 || currentDate.Day == 16;
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
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + Constants.VSMailInformTemplate;

            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, _startTime.ToString("MM/dd/yyyy hh:mm:ss"), _stopTime.ToString("MM/dd/yyyy hh:mm:ss"), TotalAffectedMembers);

            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Adjustment finished" + " (" + AppSetting.Environment + ")",
                Title = "VR Auto Mail Service",
                Body = body,
                MailingListId = mailingListId
            };

            SendMail(newMail);

            LogService.Info($"VirtualSetting service - SendMailExternal successful TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

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

            SendMail(newMail);

            LogService.Info($"VirtualSetting service - SendMailInternal successful");

            return this;
        }

        private void SendMail(Fanex.MailDelivery.Core.Models.Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }
    }
}