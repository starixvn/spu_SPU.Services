namespace Fanex.NAP.CMS.BasketballVR.Console
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

            return AppSettings.ExecutedDate.Contains(currentDate.Day);
        }

        public Service StartTime()
        {
            _startTime = DateTime.Now;

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Virtual Setting service ({AppSettings.EnvironmentName}) is running");
            }
            
            return this;
        }

        public override BaseService ExecuteForSoccer()
        {
            VirtualSettingSoccerService service = new VirtualSettingSoccerService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);

            service.AutoAdjustVirtualSettingsForPunters();

            PuntersUpdatedSoccer = service.PuntersUpdated;

            LogService.Info("ExecuteForSoccer is called.");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService ExecuteForSportbooks()
        {
            VirtualSettingSportsbookService service = new VirtualSettingSportsbookService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);

            service.AutoAdjustVirtualSettingsForPunters();

            PuntersUpdatedSportbooks = service.PuntersUpdated;

            LogService.Info("ExecuteForSportbooks is called.");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService CountAffectedMembers()
        {
            TotalAffectedMembers = PuntersUpdatedSoccer + PuntersUpdatedSportbooks;

            LogService.Info("CountAffectedMembers is called.");

            return this;
        }

        public override BaseService SendMailExternal(int mailingListId)
        {
            var environment = AppSettings.EnvironmentName;

            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + Constants.VSMailInformTemplate;

            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, _startTime.ToString("MM/dd/yyyy hh:mm:ss"), _stopTime.ToString("MM/dd/yyyy hh:mm:ss"), TotalAffectedMembers);

            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Adjustment finished" + " (" + environment + ")",
                Title = "VR Auto Mail Service",
                Body = body,
                MailingListId = mailingListId
            };

            SendMail(newMail);

            LogService.Info($"Virtual Setting service ({ environment }) - SendMailExternal successful TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Virtual Setting service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");
            }

            return this;
        }

        public override BaseService SendMailInternal(int mailingListId)
        {
            var environment = AppSettings.EnvironmentName;

            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Mail Service - Still alive" + " (" + environment + ")",
                Title = "VR Auto Mail Service - Still alive",
                Body = $"Is NOT the execution date for Updating Virtual Settings.",
                MailingListId = mailingListId
            };

            SendMail(newMail);

            LogService.Info($"Virtual Setting service ({ environment })- SendMailInternal successful");

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