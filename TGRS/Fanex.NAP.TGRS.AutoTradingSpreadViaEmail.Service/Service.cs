namespace Fanex.NAP.TGRS.AutoTradingSpreadViaEmail.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
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
            var sourceFolder = _directorySaveFile;
            var desArcFolder = AppDomain.CurrentDomain.BaseDirectory + @"Arc_Folder\";

            if (!Directory.Exists(desArcFolder))
            {
                Directory.CreateDirectory(desArcFolder);
            }

            var fileAtSourceFolders = Directory.GetFiles(sourceFolder).ToList();
            var fileAtDesArcFolders = Directory.GetFiles(desArcFolder).ToList();

            try
            {
                fileAtDesArcFolders.ForEach(filePath =>
                {
                    File.Delete(filePath);
                });

                fileAtSourceFolders.ForEach(filePath =>
                {
                    var fileName = Path.GetFileName(filePath);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        File.Move(filePath, desArcFolder + fileName);
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

            return this;
        }

        public Service GenerateAutoTradingSpread8LiveToFile(string endpoint, string saveFileName)
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
                
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return this;
        }

        public Mail GenerateMail(string saveFileName)
        {
            if (IsSuccessAPIResponse)
            {
                var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd/MMM/yyyy");
                var subject = "Automail Report - Live Performance Report " + toDate;
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
