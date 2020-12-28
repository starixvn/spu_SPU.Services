namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;    
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    public class Service
    {
        private ICustomerEvidenceService _custEvidenceService;
        private IProcessingService _processingService;
        private const string MailInformTemplate = @"MailTemplate\MailInformTemplate.cshtml";

        public Service()
        {
            _custEvidenceService = new CustomerEvidenceService();
            _processingService = new ProcessingService();
        }

        public void ProcessCustEvidenceFromFile()
        {
            try
            {
                var custEvidenceFromFiles = Enumerable.Empty<CustomerEvidenceFromFileModel>();

                do
                {
                    custEvidenceFromFiles = _custEvidenceService.GetCustomerEvidenceFromFile();

                    if (custEvidenceFromFiles != null && custEvidenceFromFiles.Any())
                    {
                        var groups = custEvidenceFromFiles.GroupBy(x => x.FileKey);

                        foreach (var custEviList in groups)
                        {
                            var isOverwrite = custEviList.ToList()[0].IsOverwrite;

                            try
                            {
                                InsertCustomerEvidence(custEviList, isOverwrite);
                            }
                            catch (Exception ex)
                            {
                                LogService.Error($"CustomerEvidence Service InsertCustomerEvidence exception: { ex.Message }", ex);
                            }
                        }

                        HandleLogError();
                    }
                } while (custEvidenceFromFiles != null && custEvidenceFromFiles.Any());
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidence Service ProcessCustEvidenceFromFile exception: { ex.Message }", ex);
            }
        }

        private void InsertCustomerEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidenceInFiles, bool isOverwrite)
        {
            var custEvidenceProcesseds = new List<CustomerEvidenceFromFileModel>();

            custEvidenceInFiles = custEvidenceInFiles.OrderBy(x => x.CTSCustID);

            var loopCustEvidences = ChunkSize(custEvidenceInFiles.ToList());

            foreach (var custEvidences in loopCustEvidences)
            {
                bool isSuccess = true;

                try
                {
                    if (isOverwrite)
                    {
                        var custEviRemovedList = Enumerable.Empty<CustomerEvidenceRemovedModel>();

                        isSuccess = _processingService.RemoveOldEvidence(custEvidences, out custEviRemovedList);
                        isSuccess = _processingService.SyncCustomerEvidenceToCTSCategory(custEvidences, 2, custEviRemovedList, isSuccess);
                    }

                    isSuccess = _processingService.InsertNewCustomerEvidence(custEvidences, isOverwrite, isSuccess);
                    isSuccess = _processingService.SyncCustomerEvidenceToCTSCategory(custEvidences, 0, isSuccess);

                    if (isSuccess)
                    {
                        custEvidenceProcesseds.AddRange(custEvidences);
                    }
                    
                    _processingService.UpdateProcessedCustEvidence(custEvidences, isSuccess);
                }
                catch (Exception ex)
                {
                    LogService.Error($"CustomerEvidence Service InsertCustomerEvidence exception: {ex.Message}", ex);
                }
            }

            if (custEvidenceProcesseds.Any())
            {
                var custClassesModel = new List<SyncCustomerClassToMainModel>();

                _processingService
                    .PushCustomerClassifyToExternal(custEvidenceProcesseds)
                    .UpdateBetLimitOfFlaggedCustomer(custEvidenceProcesseds);
            }
        }
        
        private IEnumerable<IEnumerable<CustomerEvidenceFromFileModel>> ChunkSize(List<CustomerEvidenceFromFileModel> custEvidenceInFiles)
        {
            var size = AppSettings.CustEvidenceFromFileChunkSize;

            var queue = new Queue<CustomerEvidenceFromFileModel>();
            var totalItem = custEvidenceInFiles.Count();

            for (int index = 0; index < totalItem; index++)
            {
                if (queue.Count >= size)
                {
                    var prevIndex = index - 1;

                    if (custEvidenceInFiles[index].CTSCustID == custEvidenceInFiles[prevIndex].CTSCustID)
                    {
                        queue.Enqueue(custEvidenceInFiles[index]);

                        continue;
                    }

                    var ret = queue;
                    queue = new Queue<CustomerEvidenceFromFileModel>();

                    yield return ret;
                }

                queue.Enqueue(custEvidenceInFiles[index]);
            }

            yield return queue;           
        }       

        private void HandleLogError()
        {            
            var processErrorList = _processingService.GetLogError();

            if (processErrorList.Any())
            {
                LogError(processErrorList);

                var isSendMail = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMailToSPU"].ToString());

                if (isSendMail)
                {
                    var table = "";
                    table += "<table style='" + CssStyleTable(400) + "'>";
                    table += "<tr>";
                    table += "<td style='" + CssStyleTh() + "'>Key</td>";
                    table += "<td style='" + CssStyleTh() + "'>CTSCustID</td>";
                    table += "<td style='" + CssStyleTh() + "'>EvidenceID</td>";
                    table += "<td style='" + CssStyleTh() + "'>ErrorContent</td>";
                    table += "</tr>";
                    var index = 1;

                    foreach (var item in processErrorList)
                    {
                        table += "<tr>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.FileKey + "</td>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.CTSCustID + "</td>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.EvidenceID + "</td>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.ErrorContent + "</td>";
                        table += "</tr>";

                        index++;

                        if (index == 100)
                            break;
                    }

                    table += "</table>";

                    var body = GenerateMailBody(table);

                    SendMailInternal(10278938, body);
                }               
            }
        }

        private void LogError(List<CustomerEvidenceFailedToProcessModel> processErrorList)
        {
            var sbError = new StringBuilder();

            var jsonData = JsonConvert.SerializeObject(processErrorList);

            var message = $"CustomerEvidence Service Failed: { jsonData }";

            LogService.Error(message);
        }
      
        private string CssStyleTable(int width)
        {
            var css = "border-collapse: collapse;";
            css += "width: " + width + "px";

            return css;
        }

        private string CssStyleTh()
        {
            var css = "border: 1px solid black;";
            css += "font-weight:bold;";

            return css;
        }

        private string CssStyleTd()
        {
            var css = "border: 1px solid black";

            return css;
        }

        private string GenerateMailBody(string errContent)
        {
            try
            {
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + MailInformTemplate;

                var body = Utilities.ReadTextFile(mailTemplatePath);

                return string.Format(body, errContent);
            }
            catch (Exception ex)
            {
                var message = $"CustomerEvidence Service GenerateMailBody exception: { ex.Message }";

                LogService.Error(message, ex);
            }

            return string.Empty;
        }

        private void SendMailInternal(int usertId, string body)
        {
            var subject = $"CTS service Customer-Evidence failed to process ({ AppSettings.EnvironmentName }), UserId={ usertId } at { DateTime.Now }";

            try
            {
                var mail = new Mail
                {
                    Subject = subject,
                    Title = subject,
                    Body = body,
                    MailingListId = AppSettings.MailingIdToSPU,
                    Attachments = new List<MailAttachment>()
                };

                SendMail(mail);
            }
            catch (Exception ex)
            {
                var message = $"CustomerEvidence Service InsertCustEvidence exception: { ex.Message }";

                LogService.Error(message, ex);
            }
        }

        private void SendMail(Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }

    }
}
