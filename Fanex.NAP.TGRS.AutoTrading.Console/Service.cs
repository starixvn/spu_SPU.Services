namespace Fanex.NAP.TGRS.AutoTrading.Console
{
    using Fanex.NAP.SPU.Service; 
    using System.Drawing;
    using System.IO;    
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class Service
    {
        private string _directorySaveImage;
        private LetstalkService _letstalkService;
        private string _urlTGRSAPI = "http://ha.nexdev.net:7724/TGRS/";

        public Service(string directorySaveImage)
        {
            _directorySaveImage = directorySaveImage;             
        }

        public void GenereateAutoTradingToFile(string endpoint, string saveImageName)
        {                        
            var savePath = _directorySaveImage + saveImageName;

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
                        SaveImage(byteImages, saveImageName);
                    }                    
                }
            }
        }

        public bool SendFileMessageViaLestalk()
        {
            _letstalkService = new LetstalkService("TGR-Auto-Trading", "TGR-Auto-Trading-Test", _directorySaveImage);

            return _letstalkService.SendFileMessage("AutoTrading", "AutoTrading");
        }

        private void SaveImage(byte[] byteImages, string saveImageName)
        {
            var memoryStream = new MemoryStream(byteImages);

            var image = Image.FromStream(memoryStream, true, true);

            image.Save(saveImageName);
        }
        
    }
}