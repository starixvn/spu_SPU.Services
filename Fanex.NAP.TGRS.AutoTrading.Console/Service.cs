namespace Fanex.NAP.TGRS.AutoTrading.Console
{
    using Fanex.Logging;
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
        private LetstalkService _letstalkService;
        private string _urlTGRSAPI;
        public bool IsSuccessAPIResponse { get; set; }
        #endregion

        public Service(string directorySaveImage)
        {
            _directorySaveImage = directorySaveImage;
            _urlTGRSAPI = AppSettings.TGRSAutoTradingAPIBaseAddress;
            IsSuccessAPIResponse = true;
        }

        public bool IsExecutedDate()
        {
            var currentDayOfWeek = DateTime.Today.DayOfWeek.ToString();

            //return AppSettings.TGRSAutoTradingExecutedDayOfWeek.Equals(currentDayOfWeek);

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

        public void GenereateAutoTradingToFile(string endpoint, string saveImageName)
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

                            LogService.Warn($"GenereateAutoTradingToFile response fail ({AppSettings.EnvironmentName}) - endpoint: { endpoint } : { content.ReadAsStringAsync().Result }");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuccessAPIResponse = false;

                LogService.Warn($"GenereateAutoTradingToFile fail ({AppSettings.EnvironmentName}) - endpoint: { endpoint } : { ex.ToString() }");
            }            
        }

        public bool SendFileMessageViaLestalk()
        {
            _letstalkService = new LetstalkService(AppSettings.TGRSAutoTradingLetstalkTarget
                , AppSettings.TGRSAutoTradingLetstalkGroup
                , _directorySaveImage);

            var sundayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            var subject = "Automation Trading Report";
            var content = "Good day Sir, TA report update till " + sundayOfLastWeek.ToString("dd/MM/yyyy");

            try
            {
                return _letstalkService.SendFileMessage(subject, content);
            }
            catch (Exception ex)
            {
                LogService.Warn($"SendFileMessageViaLestalk fail ({AppSettings.EnvironmentName}) - { ex.ToString() }");

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
    }
}