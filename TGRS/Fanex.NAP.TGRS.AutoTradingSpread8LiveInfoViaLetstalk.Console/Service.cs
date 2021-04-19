namespace Fanex.NAP.TGRS.AutoTradingSpread8LiveInfoViaLetstalk.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Service;    
    using System;
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

        public void GenerateAutoTradingSpread8LiveToFile(string endpoint, string saveImageName)
        {
            try
            {
                var savePath = _directorySaveImage + saveImageName;

                using (var client = new HttpClient())
                {
                    var requestUri = _urlTGRSAPI + endpoint;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync(requestUri).Result;

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

                            LogService.Warn($"GenerateAutoTradingSpread8LiveToFile response failed - endpoint: { endpoint } : { content.ReadAsStringAsync().Result }");
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
                var sundayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                var subject = "Spread 8% Live Performance Report";
                var content = "Good day Sir, Spread 8% Live Performance - Event Details 01/Jan/2019 - " + sundayOfLastWeek.ToString("dd/MM/yyyy");

                if (!string.IsNullOrEmpty(AppSettings.TGRSAutoTradingLetstalkGroup))
                {
                    var arrGroupName = AppSettings.TGRSAutoTradingLetstalkGroup.Split(';');

                    foreach (var groupName in arrGroupName)
                    {
                        var letstalkService = new LetstalkService(AppSettings.TGRSAutoTradingLetstalkTarget, groupName, _directorySaveImage);

                        if (!letstalkService.CheckExistFileAttach())
                        {
                            LogService.Warn($"SendFileMessageViaLestalk to {groupName} does not exit file attach");
                        }

                        isSendSuccess = letstalkService.SendSingleFileAttachMessage(subject, content);

                        if (!isSendSuccess)
                        {
                            LogService.Warn($"SendFileMessageViaLestalk to {groupName} failed");
                        }
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
    }
}
