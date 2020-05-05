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
        private bool _isSuccessAPIResponse;
        #endregion

        public Service(string directorySaveFile)
        {
            _directorySaveFile = directorySaveFile;
            _urlTGRSAPI = AppSettings.TGRSAutoTradingAPIBaseAddress;
            _isSuccessAPIResponse = true;
        }

        public Service MoveFileToArcFolder()
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
                LogService.Warn($"TGRS - Auto Trading: MoveFileToArcFolder ({AppSettings.EnvironmentName}) failed - { ex.ToString() }");
            }

            return this;
        }

        public Service GenerateAutoTradingToFile(string endpoint, string saveFileName)
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
                            _isSuccessAPIResponse = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _isSuccessAPIResponse = false;

                LogService.Warn($"TGRS - Auto Trading: GenereateAutoTradingToFile ({AppSettings.EnvironmentName}) failed - endpoint: { endpoint } : { ex.ToString() }");
            }

            return this;
        }

        public Service SendMailNotification(string saveFileName)
        {
            if (_isSuccessAPIResponse)
            {
                try
                {
                    var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"MailTemplate\MailInformTemplate.cshtml";
                    var body = Utilities.ReadTextFile(mailTemplatePath);
                    body = string.Format(body, saveFileName);

                    var newMail = new Mail
                    {
                        Subject = "TGRS Auto Trading report service" + " (" + AppSettings.EnvironmentName + ")",
                        Title = "TGRS Auto Trading report service",
                        Body = body,
                        Attachments = GetMailAttachments(_directorySaveFile)
                    };

                    if (AppSettings.TGRSMailingListId != -1)
                    {
                        newMail.MailingListId = AppSettings.TGRSMailingListId;

                        SendMail(newMail);
                    }

                    if (AppSettings.TGRSMailingListIdToSPU != -1)
                    {
                        newMail.MailingListId = AppSettings.TGRSMailingListIdToSPU;

                        SendMail(newMail);
                    }

                    var toDate = DateTime.Today.ToString("dd/MM/yyyy hh:mm:ss");

                    LogService.Info($"TGRS - Auto Trading: SendMailNotification ({AppSettings.EnvironmentName}) sucessfully - {toDate}");
                }
                catch (Exception ex)
                {
                    LogService.Warn($"TGRS - Auto Trading: SendMailNotification ({AppSettings.EnvironmentName}) failed - { ex.ToString() }");
                }                
            }

            return this;
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