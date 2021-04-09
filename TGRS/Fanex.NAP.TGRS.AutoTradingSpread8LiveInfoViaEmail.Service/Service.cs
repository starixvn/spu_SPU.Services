namespace Fanex.NAP.TGRS.AutoTradingSpread8LiveInfoViaEmail.Service
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.Core;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class Service
    {
        #region attributes
        private string _directorySaveFile;
        private string _urlTGRSAPI;
        public bool IsSuccessAPIResponse;
        #endregion

        public Service(string directorySaveFile)
        {
            _directorySaveFile = directorySaveFile;
            _urlTGRSAPI = AppSettings.TGRSAutoTradingAPIBaseAddress;
            IsSuccessAPIResponse = true;
        }

        public Service MoveFileToArcFolder()
        {
            return this;
        }

        public Service GenerateAutoTradingSpread8LiveToFive(string endpoint, string saveFileName)
        {
            try
            {
                var savePath = Path.Combine(_directorySaveFile + saveFileName);

                using (var client = new HttpClient())
                {
                    var requestUri = _urlTGRSAPI + endpoint;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync(requestUri).Result;

                    using (var content = response.Content)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var fileBytes = content.ReadAsByteArrayAsync().Result;

                            if (fileBytes != null)
                            {
                                using (var memoryStream = new MemoryStream(fileBytes))
                                {
                                    var excelPackage = new ExcelPackage(memoryStream);

                                    excelPackage.SaveAs(new FileInfo(savePath), AppSettings.TGRSAutoTradingFilePassword);
                                }
                            }
                        }
                        else
                        {
                            IsSuccessAPIResponse = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuccessAPIResponse = false;

                LogService.Warn($"TGRS - Auto Trading: GenereateAutoTradingToFile ({AppSettings.EnvironmentName}) failed - endpoint: { endpoint } : { ex.ToString() }");
            }

            return this;
        }

        public Mail GenerateMail(string saveFileName)
        {
            if (IsSuccessAPIResponse)
            {
                var fromDate = "07/Nov/2019";
                var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd/MMM/yyyy");
                var subject = "Automail Report - Model TA Performance Event Details - " + fromDate + " to " + toDate;
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"MailTemplate\MailInformTemplate.cshtml";

                var body = Utilities.ReadTextFile(mailTemplatePath);
                body = string.Format(body, saveFileName);

                return new Mail
                {
                    Subject = subject,
                    Title = subject,
                    Body = body,
                    Attachments = GetMailAttachments(_directorySaveFile)
                };
            }

            return null;
        }

        private List<MailAttachment> GetMailAttachments(string pathAttachment)
        {
            var attatchments = new List<MailAttachment>();

            var filePaths = Directory.GetFiles(pathAttachment).ToList();

            filePaths.ForEach(fileName =>
            {
                attatchments.Add(new MailAttachment(fileName, FileExtension.MS_EXCEL));
            });

            return attatchments;
        }
    }
}
