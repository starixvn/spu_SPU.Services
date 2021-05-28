namespace Fanex.NAP.TGRS.AutoTradingSpreadViaLetstalk.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Service;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class Service
    {
        #region attributes
        private string _directorySaveImage;
        private string _urlTGRSAPI;
        public bool IsSuccessAPIResponse { get; set; }
        #endregion

        public Service(string directorySaveImage)
        {
            _directorySaveImage = directorySaveImage;
            _urlTGRSAPI = AppSettings.TGRSAutoTradingAPIBaseAddress;
            IsSuccessAPIResponse = true;
        }

        public void MoveFileToArcFolder()
        {
            var sourceFolder = _directorySaveImage;
            var desArcFolder = AppDomain.CurrentDomain.BaseDirectory + @"Arc_Folder\";

            if (!Directory.Exists(desArcFolder))
            {
                Directory.CreateDirectory(desArcFolder);
            }

            var filePathList = Directory.GetFiles(sourceFolder).ToList();

            try
            {
                filePathList.ForEach(filePath =>
                {
                    if (File.Exists(filePath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var desFilePath = desArcFolder + fileName;
                            File.Delete(desFilePath);
                            File.Move(filePath, desFilePath);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }       

        public string GetLeagueIds(string endpoint, bool isPercent)
        {
            var leagueIds = string.Empty;

            try
            {
                using (var client = new HttpClient())
                {
                    var requestUri = _urlTGRSAPI + endpoint;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMinutes(3);

                    var contentString = new StringContent(JsonConvert.SerializeObject(isPercent), System.Text.Encoding.UTF8, "application/json");                    

                    var response = client.PostAsync(requestUri, contentString).Result;

                    using (var content = response.Content)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            leagueIds = content.ReadAsStringAsync().Result;                           
                        }
                        else
                        {
                            IsSuccessAPIResponse = false;

                            LogService.Warn($"GenerateAutoTradingSpreadToFile response failed - endpoint: { endpoint } : { content.ReadAsStringAsync().Result }");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuccessAPIResponse = false;

                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return leagueIds;
        }

        public void PostGenerateAutoTradingSpreadToFile(string endpoint, string saveImageName, IEnumerable<int> leagueIds)
        {
            try
            {
                var savePath = _directorySaveImage + saveImageName;

                using (var client = new HttpClient())
                {
                    var requestUri = _urlTGRSAPI + endpoint;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMinutes(6);

                    var contentString = new StringContent(JsonConvert.SerializeObject(leagueIds), System.Text.Encoding.UTF8, "application/json");
                    var response = client.PostAsync(requestUri, contentString).Result;

                    using (var content = response.Content)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var byteImages = content.ReadAsByteArrayAsync().Result;

                            if (byteImages != null)
                            {
                                SaveImage(byteImages, savePath);
                            }
                        }
                        else
                        {
                            IsSuccessAPIResponse = false;

                            LogService.Warn($"GenerateAutoTradingSpreadToFile response failed - endpoint: { endpoint } : { content.ReadAsStringAsync().Result }");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuccessAPIResponse = false;

                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        public bool SendFileMessageViaLestalk()
        {
            try
            {
                var isSendSuccess = true;

                if (!string.IsNullOrEmpty(AppSettings.TGRSAutoTradingLetstalkGroup))
                {
                    var arrGroupName = AppSettings.TGRSAutoTradingLetstalkGroup.Split(';');

                    foreach (var groupName in arrGroupName)
                    {
                        isSendSuccess = SendSingleFileAttachMessage(groupName);
                    }
                }

                return isSendSuccess;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return false;
            }
        }

        private void SaveImage(byte[] byteImages, string savePath)
        {
            try
            {
                var memoryStream = new MemoryStream(byteImages);

                var image = Image.FromStream(memoryStream, true, true);

                image.Save(savePath);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        private bool SendSingleFileAttachMessage(string groupName)
        {
            var isSendSuccess = true;
            var sundayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            var toDateOfLastFourWeek = sundayOfLastWeek.AddDays(-21);
            var strSundayOfLastWeek = sundayOfLastWeek.ToString("dd/MM/yyyy");
            var strToDateOfLastFourWeek = toDateOfLastFourWeek.ToString("dd/MM/yyyy");
           
            var dicMessages = new Dictionary<string, string>();
            dicMessages.Add(Consts.FileName_AutoTrading_Percent8, $"8 Points report update till { strSundayOfLastWeek }");
            dicMessages.Add(Consts.FileName_AutoTrading_Percent8LastWeek, $"8 Points by week report from { strToDateOfLastFourWeek } to { strSundayOfLastWeek }");
            dicMessages.Add(Consts.FileName_AutoTrading_Over8Percent, $"Over than 8 Points update till { strSundayOfLastWeek }");
            dicMessages.Add(Consts.FileName_AutoTrading_OverPercent8LastWeek, $"Over than 8 Points by week report update from { strToDateOfLastFourWeek } to { strSundayOfLastWeek }");
            dicMessages.Add(Consts.FileName_AutoTrading_NewTA, $"New TA model for special league (France 2 & Spain Segunda) win-lose report till { strSundayOfLastWeek }");

            foreach (var message in dicMessages)
            {
                var letstalkService = new LetstalkService(AppSettings.TGRSAutoTradingLetstalkTarget, groupName, _directorySaveImage, message.Key);

                if (!letstalkService.CheckExistFileAttach())
                {
                    LogService.Warn($"AutoTrading Spread SendFileMessageViaLestalk to {groupName} does not exit file attach");
                }
                else
                {
                    if(!letstalkService.SendSingleFileAttachMessage(string.Empty, message.Value))
                    {
                        isSendSuccess = false;
                        LogService.Warn($"AutoTrading Spread SendFileMessageViaLestalk to groupName: {groupName}, fileName: { message.Key } failed");
                    }
                }
            }            

            return isSendSuccess;
        }

    }
}
