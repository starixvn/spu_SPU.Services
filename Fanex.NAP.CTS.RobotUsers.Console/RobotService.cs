namespace Fanex.NAP.CTS.RobotUsers.Console
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class RobotService
    {
        private readonly PushDataToExternalService pushDataToExternalService;
        private readonly DataCenterService dataService;
        private IEnumerable<RobotUser> robotUsers;
        private bool isWriteLogInfo;
        private string updateExecutionTime;

        public RobotService()
        {
            pushDataToExternalService = new PushDataToExternalService();
            dataService = new DataCenterService();
        }

        public RobotService Start()
        {
            robotUsers = new List<RobotUser>();
            updateExecutionTime = DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME);
            isWriteLogInfo = AppSettings.IsWriteLogInfo;

            if (isWriteLogInfo)
                LogService.Info("RobotUsers Service - start!");

            return this;
        }

        public RobotService GetRobotUser()
        {
            var previousTimingExecution = dataService.GetPreviousTimingExecution(Constants.PARAMETER_ROBOTUSER_ID)?.SingleOrDefault();

            var fromDate = DateTime.Parse(previousTimingExecution.ParameterValue);

            robotUsers = dataService.GetRobotUser(fromDate, AppSettings.CreditSizes);

            return this;
        }

        public RobotService Process()
        {
            if (robotUsers != null && robotUsers.Any())
            {
                var robotUsersChunks = robotUsers.Chunk(AppSettings.BatchSize).ToList();

                foreach (var robotUserList in robotUsersChunks)
                {
                    var custCategory = InsertRobotUsers(robotUserList);

                    var custIds = InsertPACategory(custCategory);

                    var lastestClassification = GetLastestClassification(custIds);

                    PushDataToExternal(lastestClassification);

                    PushDataToSoon88(lastestClassification);

                    Thread.Sleep(300);
                }
            }

            return this;
        }

        public RobotService UpdateTimingExecution()
        {   
            if(!string.IsNullOrEmpty(updateExecutionTime))
            {
                dataService.UpdateTimingExecution(updateExecutionTime, Constants.PARAMETER_ROBOTUSER_ID);
            }    

            if (isWriteLogInfo)
                LogService.Info("RobotUsers Service - done!");

            return this;
        }

        private IEnumerable<LatestClassificationModel> GetLastestClassification(IEnumerable<int> custIds)
        {
            if (custIds == null || !custIds.Any())
            {
                return null;
            }

            return dataService.GetLatestClassification(string.Join(",", custIds));
        }

        private void PushDataToExternal(IEnumerable<LatestClassificationModel> lastestClassifications)
        {
            if (lastestClassifications == null || !lastestClassifications.Any())
            {
                return;
            }

            bool isSyncMainSuccessfully = pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

            if (isSyncMainSuccessfully)
            {
                dataService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

                if (isWriteLogInfo)
                    LogService.Info($"RobotUser Service - Sync CC done with :{JsonConvert.SerializeObject(lastestClassifications)} - Date: {DateTime.Now}");
            }            
        }
    

        private void PushDataToSoon88(IEnumerable<LatestClassificationModel> lastestClassifications)
        {
            if (lastestClassifications == null || !lastestClassifications.Any())
            {
                return;
            }

            pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);

            if (isWriteLogInfo)
                LogService.Info($"RobotUser Service - Push Soon88 done with :{JsonConvert.SerializeObject(lastestClassifications)}");
        }

        private InsertRobotUserResultModel InsertRobotUsers(IEnumerable<RobotUser> robotUsers)
        {
            var custCategory = new InsertRobotUserResultModel();
            var custIds = string.Empty;

            if (robotUsers != null && robotUsers.Any())
            {
                custIds = string.Join(",", robotUsers.Select(c => c.CustId));

                custCategory = dataService.InsertRobotUsers(custIds);
            }

            if (isWriteLogInfo)
                LogService.Info($"RobotUsers Service - InsertRobotUsers done! with {custIds} - Date: {DateTime.Now}");

            return custCategory;
        }

        private IEnumerable<int> InsertPACategory(InsertRobotUserResultModel custCategory)
        {
            var custs = new InsertCategoryResultModel();

            if (custCategory != null && string.IsNullOrEmpty(custCategory.ErrorMessage)
                && custCategory.CustClassification != null && custCategory.CustClassification.Any())
            {
                custs = dataService.InsertProblemAccounts(custCategory.CustClassification, false);
            }

            if (isWriteLogInfo)
            {
                LogService.Info($"RobotUsers Service - Insert PA Category done! with {custs} - Date: {DateTime.Now}");
            }

            if (!string.IsNullOrEmpty(custs.ErrorMessage))
            {
                LogService.Error($"RobotUsers Service throw exception: {custs.ErrorMessage} - Data: {JsonConvert.SerializeObject(custs.CustIds.Select(c => c))} - Date: {DateTime.Now}");
            }

            return custs.CustIds;
        }
    }
}
