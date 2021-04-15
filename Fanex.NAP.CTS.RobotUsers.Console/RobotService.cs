namespace Fanex.NAP.CTS.RobotUsers.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.Core.Constants;

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

                foreach (var robotUsers in robotUsersChunks)
                {
                    var custCategory = MappingRobotCategory(robotUsers);

                    var custIds = InsertPACategory(custCategory);

                    var lastestClassification = GetLastestClassification(custIds);

                    if (lastestClassification != null && lastestClassification.Any())
                    {
                        var tasks = new List<Task>
                        {
                            Task.Run(() =>
                            {
                                PushDataToExternal(lastestClassification);
                            }),

                            Task.Run(() =>
                            {
                                PushDataToSoon88(lastestClassification);
                            })
                        };

                        Task.WhenAll(tasks.ToArray()).Wait();
                        Thread.Sleep(300);
                    }
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

        private IEnumerable<MultipleProblemAccountsModel> MappingRobotCategory(IEnumerable<RobotUser> robotUsers)
        {
            var custCategory = new List<MultipleProblemAccountsModel>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new MultipleProblemAccountsModel
                { 
                    CustID = c.CustId, 
                    ReasonID = (int)CATEGORY.ROBOT
                }).ToList();
            }

            return custCategory;
        }

        private IEnumerable<int> InsertPACategory(IEnumerable<MultipleProblemAccountsModel> custCategory)
        {
            var custs = new InsertCategoryResultModel();

            if (custCategory != null && custCategory.Any())
            {
                custs = dataService.InsertProblemAccounts(custCategory, false);
            }

            if (isWriteLogInfo)
            {
                LogService.Info($"RobotUsers Service - Insert PA Category done! with {JsonConvert.SerializeObject(custs.CustIds)} - Date: {DateTime.Now}");
            }

            if (!string.IsNullOrEmpty(custs.ErrorMessage))
            {
                LogService.Error($"RobotUsers Service throw exception: {custs.ErrorMessage} - Data: {JsonConvert.SerializeObject(custs.CustIds)} - Date: {DateTime.Now}");
            }

            return custs.CustIds;
        }
    }
}
