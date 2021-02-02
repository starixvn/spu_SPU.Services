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

        //public LetstalkService(string targetName, string group, string pathAttachment)
        //{
        //    var letstalkApi = new ApplicationSettingServiceImp();
        //    var webAPIClientService = new WebApiClientServiceImp(letstalkApi);
        //    letstalkService = new LetstalkClientServiceImp(webAPIClientService);

        //    letstalkModel = new LetstalkModel();
        //    letstalkModel.LetstalkAttachments = GetAttachmentList(pathAttachment);
        //    letstalkModel.TargetName = targetName;
        //    letstalkModel.Group = group;
        //}

        public LetstalkService(string targetName, string group, string pathAttachment)
        {
            var letstalkApi = new ApplicationSettingServiceImp();
            var webAPIClientService = new WebApiClientServiceImp(letstalkApi);
            letstalkService = new LetstalkClientServiceImp(webAPIClientService);

            letstalkModel = new LetstalkModel();
            letstalkModel.LetstalkAttachments = new List<LetstalkAttachment>();
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
    }
}