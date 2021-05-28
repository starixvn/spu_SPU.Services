namespace Fanex.NAP.SPU.Service
{
    using Fanex.LetstalkService.Client;
    using Fanex.LetstalkService.Core.Models;
    using Fanex.LetstalkService.Core.Services;
    using Fanex.NAP.SPU.Core;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    public class LetstalkService
    {
        private LetstalkClientService letstalkService;        
        private LetstalkModel letstalkModel;

        public LetstalkService()
        {
            ApplicationSettingServiceImp letstalkApi = new ApplicationSettingServiceImp();
            WebApiClientServiceImp webAPIClientService = new WebApiClientServiceImp(letstalkApi);
            letstalkService = new LetstalkClientServiceImp(webAPIClientService);
        }

        public LetstalkService(string targetName, string group, string pathAttachment)
        {
            var letstalkApi = new ApplicationSettingServiceImp();
            var webAPIClientService = new WebApiClientServiceImp(letstalkApi);
            letstalkService = new LetstalkClientServiceImp(webAPIClientService);

            letstalkModel = new LetstalkModel();
            letstalkModel.LetstalkAttachments = GetAttachmentList(pathAttachment);
            letstalkModel.TargetName = targetName;
            letstalkModel.Group = group;
        }

        public LetstalkService(string targetName, string group, string pathAttachment, string fileName)
        {
            var letstalkApi = new ApplicationSettingServiceImp();
            var webAPIClientService = new WebApiClientServiceImp(letstalkApi);
            letstalkService = new LetstalkClientServiceImp(webAPIClientService);

            letstalkModel = new LetstalkModel();
            letstalkModel.LetstalkAttachments = GetAttachmentList(pathAttachment, fileName);
            letstalkModel.TargetName = targetName;
            letstalkModel.Group = group;
        }

        public void Info(string message)
        {
            var subject = "Info";
            SendMessage(subject, message);

        }

        public void Info(string serviceName, string message)
        {
            var subject = "Info - " + serviceName;
            SendMessage(subject, message);

        }

        public void Error(string message)
        {
            var subject = "Error";
            SendMessage(subject, message);

        }

        public void Error(string serviceName, string message)
        {
            var subject = "Error - " + serviceName;
            SendMessage(subject, message);

        }

        public bool SendFileMessage(string subject, string content)
        {
            if (letstalkModel != null)
            {
                letstalkModel.Subject = subject;
                letstalkModel.Content = content;

                return letstalkService.SendMessage(letstalkModel);
            }

            return false;
        }

        public bool CheckExistFileAttach()
        {
            if (letstalkModel != null && letstalkModel.LetstalkAttachments.Any())
            {
                return true;
            }

            return false;
        }

        public bool SendSingleFileAttachMessage(string subject, string content)
        {
            if (letstalkModel != null && letstalkModel.LetstalkAttachments.Any())
            {
                var countSuccess = 0;
                var model = new LetstalkModel
                {
                    Subject = subject,
                    Content = content,
                    LetstalkAttachments = new List<LetstalkAttachment> { letstalkModel.LetstalkAttachments[0] },
                    TargetName = letstalkModel.TargetName,
                    Group = letstalkModel.Group
                };

                var isSuccess = letstalkService.SendMessage(model);
                countSuccess = isSuccess ? countSuccess + 1 : countSuccess;
                
                var countAttachment = letstalkModel.LetstalkAttachments.Count;
                for (int i = 1; i < letstalkModel.LetstalkAttachments.Count; i++)
                {
                    model.LetstalkAttachments = new List<LetstalkAttachment> { letstalkModel.LetstalkAttachments[i] };
                    model.Subject = string.Empty;
                    model.Content = string.Empty;                   

                    isSuccess = letstalkService.SendMessage(model);
                    countSuccess = isSuccess ? countSuccess + 1 : countSuccess;
                };

                return countSuccess == countAttachment;
            }

            return false;
        }

        private void SendMessage(string subject, string content)
        {
            var env = string.Empty;

            switch (AppSettings.Environment)
            {
                case 3:
                    env = "PRO";
                    break;
                case 2:
                    env = "UAT";
                    break;
                default:
                    env = "LOCAL";
                    break;
            }

            LetstalkModel messsage = new LetstalkModel()
            {
                TargetName = AppSettings.LetstalkTarget,
                Subject = subject + " - " + env,
                Content = content,
                Group = AppSettings.LetstalkGroup,
                LetstalkAttachments = new List<LetstalkAttachment>()
            };

            letstalkService.SendMessage(messsage);
        }

        private List<LetstalkAttachment> GetAttachmentList(string pathAttachment)
        {
            var attatchment = new List<LetstalkAttachment>();            

            var filePaths = Directory.GetFiles(pathAttachment).ToList();

            filePaths.ForEach(fileName =>
            {
                attatchment.Add(new LetstalkAttachment
                {
                    FileName = fileName,
                    Bytes = FileBinaryService.ConvertFileToBinary(fileName)
                });
            });

            return attatchment;
        }

        private List<LetstalkAttachment> GetAttachmentList(string pathAttachment, string fileName)
        {
            var attatchment = new List<LetstalkAttachment>();

            var filePaths = Directory.GetFiles(pathAttachment).ToList();

            filePaths.ForEach(file =>
            {
                var xfileName = Path.GetFileName(file);

                if (xfileName == fileName)
                {
                    attatchment.Add(new LetstalkAttachment
                    {
                        FileName = file,
                        Bytes = FileBinaryService.ConvertFileToBinary(file)
                    });
                }        
            });

            return attatchment;
        }

    }
}
