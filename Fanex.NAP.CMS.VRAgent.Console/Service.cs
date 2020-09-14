namespace Fanex.NAP.CMS.VRAgent.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;    

    public class Service : BaseService
    {
        private const string VSMailInformNoAgentAffectTemplate = @"MailTemplate\MailInformNoAgentAffectTemplate.cshtml";
        private const string VSMailInformTemplate = @"MailTemplate\MailInformTemplate.cshtml";

        #region Properties
        private DateTime _startTime;

        private DateTime _stopTime;

        private IEnumerable<VRAgentUpdatedToZeroModel> _updatedToZeroList;

        private IEnumerable<VRAgentUpdatedToFollowSuperModel> _updatedToFollowSupperList;
        #endregion

        public Service StartTime()
        {
            _startTime = DateTime.Now;
            LogService.Info($"VR - Auto Agent Adjustment service - StartTime: {_startTime}.");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"VR - Auto Agent Adjustment service ({AppSettings.EnvironmentName}) is running");
            }

            return this;
        }

        public override BaseService UpdateVRAgent()
        {
            var service = new VRAgentService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    service.AutoVRAgentAdjustment();
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Auto Agent Adjustment service - UpdateVRAgent exception with countRetry: {countRetry} message:  {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            TotalAffectedMembers = service.PuntersUpdated;

            LogService.Info("VR - Auto Agent Adjustment service - UpdateVRAgent successfully!");

            (_updatedToZeroList, _updatedToFollowSupperList) = service.GetVRAgentUpdated(_startTime);

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService NotifyMessage()
        {
            var environment = AppSettings.EnvironmentName;
            
            var message = $"VR - Auto Agent Adjustment service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }.";

            if (_updatedToZeroList != null)
            {
                message += $" Total Agents updated to 0: {_updatedToZeroList.Count() }.";
            }

            if (_updatedToFollowSupperList != null)
            {
                message += $" Total Agents updated to follow Super: {_updatedToFollowSupperList.Count() }.";
            }

            LogService.Info(message);

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info(message);
            }

            return this;
        }

        public override BaseService SendMailExternal(int mailingListId)
        {
            var subject = "List of new agents got VR updated by Agent profiling job";
            var body = GenerateMailBody();

            try
            {
                var mail = new Mail
                {
                    Subject = subject,
                    Title = subject,
                    Body = body,
                    MailingListId = mailingListId,
                    Attachments = new List<MailAttachment>()
                };

                SendMail(mail);
            }
            catch (Exception ex)
            {
                var message = $"VR - Auto Agent Adjustment service - SendMailExternal exception: { ex.Message } with body content: { body }";

                LogService.Error(message, ex);
            }

            return this;
        }

        private string GenerateMailBody()
        {
            var body = string.Empty;

            try
            {
                var isExistAgent = _updatedToZeroList != null && _updatedToZeroList.Any()
               || _updatedToFollowSupperList != null && _updatedToFollowSupperList.Any();

                var mailTemplatePath = isExistAgent
                    ? AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate
                    : AppDomain.CurrentDomain.BaseDirectory + VSMailInformNoAgentAffectTemplate;

                body = Utilities.ReadTextFile(mailTemplatePath);

                var html = string.Empty;

                html += GenerateTableUpdateToZero();
                html += GenerateTableUpdateToFollowSupper();

                body = string.Format(body, html);
            }
            catch (Exception ex)
            {
                var message = $"VR - Auto Agent Adjustment service - GenerateMailBody exception: { ex.Message }";

                LogService.Error(message, ex);
            }
           
            return body;
        }

        private string GenerateTableUpdateToZero()
        {
            var table = "</br>Table: agents whose VR got updated to 0 </br></br>";

            if (_updatedToZeroList != null && _updatedToZeroList.Any())
            {
                table += "<table style='" + CssStyleTable(300) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Agent</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in _updatedToZeroList)
                {
                    table += "<tr>";
                    table += "<td style='" + CssStyleTd() + "'>" + index + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Site + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Agent + "</td>";
                    table += "</tr>";

                    index++;
                }

                table += "</table></br>";
            }

            return table;
        }

        private string GenerateTableUpdateToFollowSupper()
        {
            var table = "</br>Table: agents whose VR got updated to follow Super </br></br>";

            if (_updatedToFollowSupperList != null && _updatedToFollowSupperList.Any())
            {
                table += "<table style='" + CssStyleTable(400) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Agent</td>";
                table += "<td style='" + CssStyleTh() + "'>Member's Winloss</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in _updatedToFollowSupperList)
                {
                    table += "<tr>";
                    table += "<td style='" + CssStyleTd() + "'>" + index + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Site + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Agent + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.MemberWinloss.ToString("0") + "RM" + "</td>";
                    table += "</tr>";

                    index++;
                }

                table += "</table>";
            }

            return table;
        }

        private string CssStyleTable(int width)
        {
            var css = "border-collapse: collapse;";
            css += "width: " + width + "px";

            return css;
        }

        private string CssStyleTh()
        {
            var css = "border: 1px solid black;";
            css += "font-weight:bold;";

            return css;
        }

        private string CssStyleTd()
        {
            var css = "border: 1px solid black";

            return css;
        }

        private void SendMail(Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }       

    }
}
