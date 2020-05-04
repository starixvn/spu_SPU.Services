namespace Fanex.NAP.TGRS.AutoTradingViaEmail.Console
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.Core;
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
        public bool IsSuccessAPIResponse { get; set; }
        #endregion

        public Service(string directorySaveFile)
        {
            _directorySaveFile = directorySaveFile;
            _urlTGRSAPI = AppSettings.TGRSAutoTradingAPIBaseAddress;
            IsSuccessAPIResponse = true;
        }

        public void MoveFileToArcFolder()
        {
            var sourceFolder = _directorySaveFile;
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
                LogService.Warn($"MoveFileToArcFolder fail ({AppSettings.EnvironmentName}) - { ex.ToString() }");
            }
        }

        public void GenereateAutoTradingToFile(string endpoint, string saveFileName)
        {
            var savePath = Path.Combine(_directorySaveFile + saveFileName);

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
                            var fileBytes = content.ReadAsByteArrayAsync().Result;

                            if (fileBytes != null)
                            {
                                using (var memoryStream = new MemoryStream(fileBytes))
                                {
                                    using(var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                                    {
                                        memoryStream.WriteTo(fileStream);
                                    }
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

                LogService.Warn($"GenereateAutoTradingToFile fail ({AppSettings.EnvironmentName}) - endpoint: { endpoint } : { ex.ToString() }");
            }
        }

        public void SendMailInternal(int mailingListId, string saveFileName)
        {
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"MailTemplate\MailInformTemplate.cshtml";

            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, saveFileName);            

            var newMail = new Mail
            {
                Subject = "TGRS Auto Trading report service" + " (" + AppSettings.EnvironmentName + ")",
                Title = "TGRS Auto Trading report service",
                Body = body,
                MailingListId = mailingListId,
                Attachments = GetMailAttachments(_directorySaveFile)
            };

            SendMail(newMail);
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

        private void SendMail(Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }

    }
}