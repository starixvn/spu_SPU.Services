namespace Fanex.NAP.TGRS.AutoTrading.Console
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Service;
    using System;    
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class Service
    {      
        private string _directorySaveImage;
        private LetstalkService _letstalkService;
        private string _urlTGRSAPI;

        public Service(string directorySaveImage)
        {
            _directorySaveImage = directorySaveImage;
            _urlTGRSAPI = AppSettings.TGRSAutoTradingAPIBaseAddress;
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
                LogService.Warn($"MoveFileToArcFolder ({AppSettings.EnvironmentName}) - { ex.Message }");
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

                    using (HttpContent content = response.Content)
                    {
                        var byteImages = content.ReadAsByteArrayAsync().Result;

                        if (byteImages != null)
                        {
                            SaveImage(byteImages, savePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Warn($"GenereateAutoTradingToFile ({AppSettings.EnvironmentName}) - { ex.Message }");
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
                LogService.Warn($"SendFileMessageViaLestalk ({AppSettings.EnvironmentName}) - { ex.Message }");

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
                LogService.Warn($"SaveImage ({AppSettings.EnvironmentName}) - ({savePath}) - { ex.Message }");
            }            
        }      
    }
}