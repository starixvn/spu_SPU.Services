namespace Fanex.NAP.TGRS.AutoTrading.Console
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Service;
    using Newtonsoft.Json;
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

        public bool IsDateExecute()
        {
            var toDate = DateTime.Today.ToString("MM/dd/yyyy");
            var strSpecialDate = AppSettings.TGRSAutoTradingSpecicalDateExecute;

            if (!string.IsNullOrEmpty(strSpecialDate))
            {
                var specialDate = DateTime.Parse(strSpecialDate);
                if (toDate.Equals(specialDate.ToString("MM/dd/yyyy")))
                {
                    return true;
                }

                return false;
            }

            return true;
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
            catch(Exception ex)
            {
                LogService.Warn($"MoveFileToArcFolder fail ({AppSettings.EnvironmentName}) - { ex.ToString() }");
            }
        }

        public void GenerateAutoTradingToFile(string endpoint, string saveImageName)
        {                        
            var savePath = _directorySaveImage + saveImageName;

            try
            {
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

                            LogService.Warn($"GenereateAutoTradingToFile ({AppSettings.EnvironmentName}) response failed - endpoint: { endpoint } : { content.ReadAsStringAsync().Result }");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuccessAPIResponse = false;

                LogService.Warn($"GenereateAutoTradingToFile ({AppSettings.EnvironmentName}) failed - endpoint: { endpoint } : { ex.ToString() }");
            }            
        }

        public bool SendFileMessageViaLestalk()
        {
            try
            {
                var isSendSuccess = true;
                var sundayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                var subject = "Automation Trading Report";
                var content = "Good day Sir, TA report update till " + sundayOfLastWeek.ToString("dd/MM/yyyy");

                if (!string.IsNullOrEmpty(AppSettings.TGRSAutoTradingLetstalkGroup))
                {
                    var arrGroupName = AppSettings.TGRSAutoTradingLetstalkGroup.Split(';');

                    foreach (var groupName in arrGroupName)
                    {
                        var letstalkService = new LetstalkService(AppSettings.TGRSAutoTradingLetstalkTarget, groupName, _directorySaveImage);

                        if (!letstalkService.SendFileMessage(subject, content))
                        {
                            isSendSuccess = false;

                            LogService.Warn($"SendFileMessageViaLestalk ({AppSettings.EnvironmentName}) to {groupName} failed");
                        }
                    }
                }
                                
                return isSendSuccess;
            }
            catch (Exception ex)
            {
                LogService.Warn($"SendFileMessageViaLestalk failed ({AppSettings.EnvironmentName}) - { ex }");

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
                LogService.Warn($"SaveImage fail ({AppSettings.EnvironmentName}) - save path: ({savePath}) : { ex.ToString() }");
            }            
        }      

        public void SendMessageViaLetstalkTest()
        {
            try
            {
                var subject = "Testing";
                var content = "Testing";

                if (!string.IsNullOrEmpty(AppSettings.TGRSAutoTradingLetstalkGroup))
                {
                    var arrGroupName = AppSettings.TGRSAutoTradingLetstalkGroup.Split(';');

                    foreach (var groupName in arrGroupName)
                    {
                        var letstalkService = new LetstalkService(AppSettings.TGRSAutoTradingLetstalkTarget, groupName, _directorySaveImage);

                        if (!letstalkService.SendFileMessage(subject, content))
                        {                         
                            LogService.Warn($"SendMessageViaLetstalkTest ({AppSettings.EnvironmentName}) to {groupName} failed");
                        }
                        else
                        {
                            LogService.Info($"SendMessageViaLetstalkTest ({AppSettings.EnvironmentName}) to {groupName} done");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Warn($"SendMessageViaLetstalkTest failed ({AppSettings.EnvironmentName}) - { ex.ToString() }");                
            }
        }
    }
}