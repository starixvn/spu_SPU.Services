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
        private const string VSMailInformNoMemberAffectTemplate = @"MailTemplate\MailInformNoMemberAffectTemplate.cshtml";
        private const string VSMailInformTemplate = @"MailTemplate\MailInformTemplate.cshtml";

        private readonly VRMemberBigStakeService memberBigStakeService;

        private VRGetMemberBigStakePerformanceModel getMemberBigStakePerformanceModel;

        public Service()
        {
            memberBigStakeService = new VRMemberBigStakeService();
        }

        public Service StartTime()
        {
            LogService.Info($"VR - Update Member Big Stake - StartTime: {DateTime.Now}.");

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

        public override BaseService SendMailExternal(int mailingListId)
        {
            var subject = "List of member big stake got VR updated by profiling job";
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
                var isExistMember = (getMemberBigStakePerformanceModel.FirstBetDateList != null && getMemberBigStakePerformanceModel.FirstBetDateList.Any())
                               || (getMemberBigStakePerformanceModel.StatusList != null && getMemberBigStakePerformanceModel.StatusList.Any());

                var mailTemplatePath = isExistMember
                    ? AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate
                    : AppDomain.CurrentDomain.BaseDirectory + VSMailInformNoMemberAffectTemplate;

                body = Utilities.ReadTextFile(mailTemplatePath);

                var html = string.Empty;

                html += GenerateContentFirstBetDate();
                html += GenerateContentStatus();

                body = string.Format(body, html);
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

        private string GenerateContentFirstBetDate()
        {
            var content = string.Empty;

            if (getMemberBigStakePerformanceModel.FirstBetDateList != null && getMemberBigStakePerformanceModel.FirstBetDateList.Any())
            {
                content = $"<br />List {getMemberBigStakePerformanceModel.FirstBetDateList.Count()} member(s) big stake [first bet date] whose VR got updated: <br />";
                
                content += JsonConvert.SerializeObject(getMemberBigStakePerformanceModel.FirstBetDateList);
            }

            return content;
        }

        private string GenerateContentStatus()
        {
            var content = string.Empty;

            if (getMemberBigStakePerformanceModel.StatusList != null && getMemberBigStakePerformanceModel.StatusList.Any())
            {
                content = $"<br /><br />List {getMemberBigStakePerformanceModel.StatusList.Count()} member(s) big stake [status] whose VR got updated: <br />";

                content += JsonConvert.SerializeObject(getMemberBigStakePerformanceModel.StatusList);
            }

            return content;
        }

        private void SendMail(Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }
    }
}
