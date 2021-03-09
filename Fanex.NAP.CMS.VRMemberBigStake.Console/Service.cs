namespace Fanex.NAP.CMS.VRMemberBigStake.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Service : BaseService
    {
        private const string MailInformNoMemberAffectTemplate = @"MailTemplate\MailInformNoMemberAffectTemplate.cshtml";
        private const string MailInformBigCreditMemberTemplate = @"MailTemplate\MailInformBigCreditMemberTemplate.cshtml";
        private const string MailInformBigLossTemplate = @"MailTemplate\MailInformBigLossTemplate.cshtml";
        private const string MailInformBigCreditMemberAndBigLossTemplate = @"MailTemplate\MailInformBigCreditMemberAndBigLossTemplate.cshtml";

        private const string FormatDecimalToString_Place = "#,##0.00";

        private readonly VRMemberBigStakeService memberBigStakeService;

        private VRGetMemberBigStakePerformanceModel getMemberBigStakePerformanceModel;

        public Service()
        {
            memberBigStakeService = new VRMemberBigStakeService();
        }

        public Service StartTime()
        {
            LogService.Info($"VR - Update Member Big Stake - START TIME: {DateTime.Now}.");

            return this;
        }

        public Service EndTime()
        {
            LogService.Info($"VR - Update Member Big Stake - END TIME: {DateTime.Now}.");

            return this;
        }

        public Service InsertVRMemberBigStake()
        {
            memberBigStakeService.InsertVRMemberBigStake();

            return this;
        }

        public Service GetMemberBigStakePerformance()
        {
            getMemberBigStakePerformanceModel = memberBigStakeService.GetMemberBigStakePerformance();

            return this;
        }

        public Service UpdateMemberBigStakeFirstBetDate()
        {
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    if (getMemberBigStakePerformanceModel != null && getMemberBigStakePerformanceModel.FirstBetDateList != null && getMemberBigStakePerformanceModel.FirstBetDateList.Any())
                    {
                        getMemberBigStakePerformanceModel.FirstBetDateList.GroupBy(x => x.FirstBetDate).ToList().ForEach(r =>
                        {
                            var custIds = r.Select(x => x.CustId);

                            custIds.ChunkSize(500).ToList().ForEach(x =>
                            {
                                memberBigStakeService.UpdateVRMemeberBigStakeFirstBetDate(x, 1, r.Key);
                            });
                        });

                        var json = JsonConvert.SerializeObject(getMemberBigStakePerformanceModel.FirstBetDateList);

                        LogService.Info($"VR - List { getMemberBigStakePerformanceModel.FirstBetDateList.Count() } member(s) [First Bet Date Member] update Big Stake: {json}");
                    }

                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Update First Bet Date Member Big Stake - Retry: {countRetry} message:  {logMessage}");
                    
                    countRetry += 1;

                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw;
                    }
                }
            }

            return this;
        }

        public Service UpdateMemberBigStakeStatus()
        {

            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    if (getMemberBigStakePerformanceModel != null && getMemberBigStakePerformanceModel.StatusList != null && getMemberBigStakePerformanceModel.StatusList.Any())
                    {
                        var custIdByStatus = getMemberBigStakePerformanceModel.StatusList.Select(x => x.CustId);

                        custIdByStatus.ChunkSize(500).ToList().ForEach(x => {
                            memberBigStakeService.UpdateVRWhitelistMemberBigStake(custIdByStatus, 1);
                        });

                        var json = JsonConvert.SerializeObject(getMemberBigStakePerformanceModel.StatusList);

                        LogService.Info($"VR - List { getMemberBigStakePerformanceModel.StatusList.Count() } member(s) [Status] update Member Big Stake: {json}");
                    }

                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Update White list Member Big Stake Status - Retry: {countRetry} message:  {logMessage}");

                    countRetry += 1;

                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw;
                    }
                }
            }            
               
            return this;
        }        

        public Service SendMail(int mailingListId)
        {
            if (mailingListId <= 0)
            {
                return this;
            }

            var subject = DateTime.Now.ToString("yyyyMMdd") + " List of big credit members just having their first ticket.";
            var body = GenerateMailBody();

            try
            {
                if (!string.IsNullOrEmpty(body))
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
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - SendMailExternal exception: message:  {logMessage}");
            }

            return this;
        }        

        private string GenerateMailBody()
        {
            var body = string.Empty;

            try
            {
                var creditMemberBigStake = memberBigStakeService.GetBigCreditMemberForAutomail();
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory;
                var table1 = string.Empty;
                var table2 = string.Empty;

                if (!creditMemberBigStake.BigCreditMemberList.Any() && !creditMemberBigStake.BigCreditMemberWithBigLossList.Any())
                {
                    mailTemplatePath += MailInformNoMemberAffectTemplate;

                }
                else if(creditMemberBigStake.BigCreditMemberList.Any() && !creditMemberBigStake.BigCreditMemberWithBigLossList.Any())
                {
                    mailTemplatePath += MailInformBigCreditMemberTemplate;

                    table1 = GenerateTable(creditMemberBigStake.BigCreditMemberList);
                }
                else if(!creditMemberBigStake.BigCreditMemberList.Any() && creditMemberBigStake.BigCreditMemberWithBigLossList.Any())
                {
                    mailTemplatePath += MailInformBigLossTemplate;
                    table1 = GenerateTable(creditMemberBigStake.BigCreditMemberWithBigLossList);
                }
                else
                {
                    mailTemplatePath += MailInformBigCreditMemberAndBigLossTemplate;

                    table1 = GenerateTable(creditMemberBigStake.BigCreditMemberList);

                    table2 = GenerateTable(creditMemberBigStake.BigCreditMemberWithBigLossList);
                }

                body = Utilities.ReadTextFile(mailTemplatePath);                                                             

                body = string.Format(body, table1, table2);

                var json = JsonConvert.SerializeObject(creditMemberBigStake);

                LogService.Info($"VR - List member(s) Big Stake for send mail: {json}");
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - GenerateMailBody exception: message:  {logMessage}");
            }

            return body;
        }

        private string GenerateTable(IEnumerable<VRAutomailBigCreditMemberModel> creditMemberBigStake)
        {
            var table = string.Empty;

            if (creditMemberBigStake != null && creditMemberBigStake.Any())
            {
                table += "<table style='" + CssStyleTable(700) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No.</td>";
                table += "<td style='" + CssStyleTh() + "'>Member</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Given Credit (RM)</td>";
                table += "<td style='" + CssStyleTh() + "'>Member Turnover (RM)</td>";
                table += "<td style='" + CssStyleTh() + "'>Member Winloss (RM)</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in creditMemberBigStake)
                {
                    table += "<tr>";
                    table += "<td style='" + CssStyleTd() + "'>" + index + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Member + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Site + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.GivenCredit_RM.ToString(FormatDecimalToString_Place) + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.MemberTurnover_RM.ToString(FormatDecimalToString_Place) + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.MemberWinloss_RM.ToString(FormatDecimalToString_Place) + "</td>";
                    table += "</tr>";

                    index++;
                }

                table += "</table></br>";
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
