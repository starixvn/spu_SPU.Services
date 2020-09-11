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
            //LogService.Info("Start executing for VR - Auto Agent Adjustment service.");

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

                    LogService.Error($"VR - Auto Agent Adjustment service - Start Exception {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            (_updatedToZeroList, _updatedToFollowSupperList) = service.GetVRAgentUpdated(_startTime);

            TotalAffectedMembers = service.PuntersUpdated;

            //LogService.Info("Executed for VR - Auto Agent Adjustment successfully!");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService NotifyMessage()
        {
            var environment = AppSettings.EnvironmentName;

            //LogService.Info($"VR - Auto Agent Adjustment service ({ environment }) - TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"VR - Auto Agent Adjustment service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");
            }

            return this;
        }

        public override BaseService SendMailExternal(int mailingListId)
        {
            var subject = "List of new agents got VR updated by Agent profiling job";
            var body = GenerateMailBody();

            var mail = new Mail
            {
                Subject = subject,
                Title = subject,
                Body = body,
                MailingListId = mailingListId,
                Attachments = new List<MailAttachment>()
            };

            SendMail(mail);

            return this;
        }

        private string GenerateMailBody()
        {
            //var isExistAgent = _updatedToZeroList != null && _updatedToZeroList.Any()
            //    || _updatedToFollowSupperList != null && _updatedToFollowSupperList.Any();

            var isExistAgent = true;

            var mailTemplatePath = isExistAgent
                ? AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate
                : AppDomain.CurrentDomain.BaseDirectory + VSMailInformNoAgentAffectTemplate;

            var body = Utilities.ReadTextFile(mailTemplatePath);
            
            var html = string.Empty;
            
            html += GenerateTableUpdateToZero();
            html += GenerateTableUpdateToFollowSupper();

            body = string.Format(body, html);

            return body;
        }

        private string GenerateTableUpdateToZero()
        {
            var table = "</br>Table: agents whose VR got updated to 0 </br></br>";

            _updatedToZeroList = GenerateDataForTest();

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

            _updatedToFollowSupperList = GenerateDataForTest2();

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

        private IEnumerable<VRAgentUpdatedToZeroModel> GenerateDataForTest()
        {
            var list = new List<VRAgentUpdatedToZeroModel>();

            for (int i = 0; i < 40; i++)
            {
                list.Add(new VRAgentUpdatedToZeroModel
                {
                    Agent = "acbde1" + i.ToString(),
                    Site = "755555" + i.ToString()
                });
            }

            return list;
        }

        private IEnumerable<VRAgentUpdatedToFollowSuperModel> GenerateDataForTest2()
        {
            var list = new List<VRAgentUpdatedToFollowSuperModel>();

            for (int i = 1; i < 40; i++)
            {
                list.Add(new VRAgentUpdatedToFollowSuperModel
                {
                    Agent = "acbde1" + i.ToString(),
                    Site = "755555" + i.ToString(),
                    MemberWinloss = new decimal(1.26 + i)
                });
            }
          
            return list;
        }

    }
}
