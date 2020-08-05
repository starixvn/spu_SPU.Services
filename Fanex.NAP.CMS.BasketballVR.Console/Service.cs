namespace Fanex.NAP.CMS.BasketballVR.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;

    public class Service : BaseService
    {
        private DateTime _startTime;

        private DateTime _stopTime;

        public Service StartTime()
        {
            _startTime = DateTime.Now;

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Basketball - Virtual Setting service ({AppSettings.EnvironmentName}) is running");
            }
            
            return this;
        }

        public override BaseService ExecuteForBasketball()
        {
            VirtualSettingBasketballService service = new VirtualSettingBasketballService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    service.AutoAdjustVirtualSettingsForPunters();
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"Basketball - VirtualSetting service - Start Exception {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            TotalAffectedMembers = service.PuntersUpdated;

            LogService.Info("Execute For Basketball is called.");

            _stopTime = DateTime.Now;

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
                Subject = "Basketball - VR Auto Adjustment finished" + " (" + environment + ")",
                Title = "Basketball - VR Auto Mail Service",
                Body = body,
                MailingListId = mailingListId
            };

            SendMail(newMail);

            LogService.Info($"Basketball - Virtual Setting service ({ environment }) - SendMailExternal successful TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Basketball - Virtual Setting service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");
            }

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