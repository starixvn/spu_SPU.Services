namespace Fanex.NAP.CTS.CustomerClassification.AutoTag
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.XML;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.Core.Constants;

    public class Service
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly IPushDataToExternalService _pushDataToExternalService;

        public Service()
        {
            _dataCenterService = new DataCenterService();
            _pushDataToExternalService = new PushDataToExternalService();
        }

        public void Process()
        {
            try
            {
                var newClassification = new List<CustomerClassificationModelMoreInfo>();

                do
                {
                    newClassification = _dataCenterService.GetNewClassification();

                    if (AppSettings.IsWriteLogInfo)  
                        LogService.Info($"Normal.Service - Get classification: {JsonConvert.SerializeObject(newClassification)} - Time: {DateTime.Now}");

                    if (newClassification != null && newClassification.Any())
                    {
                        var generalClassifications = newClassification.Where(c => c.IsGeneralCategory);

                        if (generalClassifications != null && generalClassifications.Any())
                        {
                            InsertGeneralClassificationCusts(generalClassifications);
                        }
                    }
                } while (newClassification != null && newClassification.Any());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        private void InsertGeneralClassificationCusts(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification)
        {
            var newGeneralClassificationChunk = generalClassification.Chunk(AppSettings.BatchSize).ToList();

            foreach (var listClassification in newGeneralClassificationChunk)
            {
                try
                {
                    //1.1. Check Association With PA
                    var custIds = this.GetCustIds(listClassification);

                    var customerAssociationProblemAccounts = _dataCenterService.GetCustomerAssociationProblemAccount(custIds);

                    if (AppSettings.IsWriteLogInfo) 
                        LogService.Info($"Normal.Service - Get PA Ass: {JsonConvert.SerializeObject(customerAssociationProblemAccounts)} - Time: {DateTime.Now}");

                    var jsonClassification = MergeCustomerClassificationInfo(listClassification, customerAssociationProblemAccounts);

                    // 1.2. Insert data in CTS
                    if (AppSettings.IsWriteLogInfo) 
                        LogService.Info($"Normal.Service - InputMain Data {jsonClassification} - Time: {DateTime.Now}");
                    
                    var typeCustModel = _dataCenterService.InsertMainCustomerClassification(jsonClassification);

                    if (IsInsertMainSuccessful(typeCustModel.ErrorMessage, listClassification))
                    {
                        //2. Push Data to External
                        var isSuccess = PushDataToExternal(typeCustModel?.CustIds.ToList());

                        // 3. Update IsApplied in mainDB
                        if(isSuccess)
                        {
                            _dataCenterService.UpdateAppliedStatusClassification(custIds, true);

                            if (AppSettings.IsWriteLogInfo)
                                LogService.Info($"Normal.Service - update IsApplied DW done - Time: {DateTime.Now}");
                        }    
                    }
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($" {logMessage}. Failed with data {JsonConvert.SerializeObject(listClassification)}");
                }
            }
        }

        private bool PushDataToExternal(List<int> custList)
        {
            if (custList == null || !custList.Any())
            {
                return true;
            }

            var latestCustIdsChunk = custList.Chunk(AppSettings.PushCCToExternalChunkSize);
            foreach (var custIds in latestCustIdsChunk)
            {
                var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",", custIds));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSuccess = UpdateCCToMainDB(lastestClassifications);
                    if (AppSettings.IsWriteLogInfo)
                        LogService.Info($"Normal.Service - Push CC {(isSuccess ? "done" : "fail")} {JsonConvert.SerializeObject(lastestClassifications)}- Time: {DateTime.Now}");

                    if (isSuccess)
                    {
                        isSuccess = UpdateDangerLevelToMainDB(lastestClassifications);
                    }

                    if (isSuccess)
                    {
                        isSuccess = _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                        if (AppSettings.IsWriteLogInfo) LogService.Info($"Normal.Service - PushSoon88 {JsonConvert.SerializeObject(lastestClassifications)}- Time: {DateTime.Now}");
                    }

                    if (!isSuccess)
                        return false;
                }
            }

            return true;
        }

        private bool UpdateCCToMainDB(IEnumerable<LatestClassificationModel> lastestCC)
        {
            if (lastestCC == null || !lastestCC.Any())
            {
                return true;
            }

            var isSuccessToMain = _pushDataToExternalService.PushCustomerClassificationToMain(lastestCC, 2);

            if (isSuccessToMain)
            {
                var updateApplyCCs = lastestCC.Select(x => new { x.CustId, x.ScannedTime });
                var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                _dataCenterService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);
            }

            return isSuccessToMain;
        }

        private bool UpdateDangerLevelToMainDB(IEnumerable<LatestClassificationModel> lastestCC)
        {
            if (lastestCC == null || !lastestCC.Any())
            {
                return true;
            }

            var dangerLevels = _dataCenterService.GetDangerLevel(string.Join(",", lastestCC.Select(c=>c.CustId)), DANGERLEVEL.ORIGINAL);
            if (AppSettings.IsWriteLogInfo) 
                LogService.Info($"Normal.Service - Get Danger {JsonConvert.SerializeObject(dangerLevels)}- Time: {DateTime.Now}");

            if (dangerLevels != null && dangerLevels.Any())
            {
                var isUpdateSuccessful = _pushDataToExternalService.UpdateDangerLevelToMain(dangerLevels, DANGERLEVEL.ORIGINAL, 2);
                if (AppSettings.IsWriteLogInfo) 
                    LogService.Info($"Normal.Service - Update danger level {(isUpdateSuccessful ? "done" : "fail")} with [CustList: {dangerLevels.ToXmlString()}, DangerType: { JsonConvert.SerializeObject((int)DANGERLEVEL.ORIGINAL)}] - Time: {DateTime.Now}");

                if (isUpdateSuccessful)
                {
                    var updateApplyCCs = dangerLevels.Select(x => new { x.CustId, x.ScannedTime });
                    var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                    _dataCenterService.UpdateAppliedDangerLevel(jsonUpdateApplyCCs, DANGERLEVEL.ORIGINAL);
                }

                return isUpdateSuccessful;
            }

            return true;
        }

        private string GetCustIds(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification)
        {
            if (generalClassification != null && generalClassification.Any())
            {
                var custIds = generalClassification.Select(x => x.CustId).Distinct();

                return string.Join(",", custIds);
            }

            return string.Empty;
        }

        private string MergeCustomerClassificationInfo(IEnumerable<CustomerClassificationModelMoreInfo> custClassification, IEnumerable<CustomerAssociationProblemAccountModel> custAssociationPA)
        {
            if (custAssociationPA == null || !custAssociationPA.Any()
                || custClassification == null || !custClassification.Any())
            {
                return string.Empty;
            }

            var result = custClassification.GroupJoin(custAssociationPA,
                               custClass => custClass.CustId,
                               custAss => custAss.CustID,
                               (custClass, custPA) => new { custClass, custPA })
                               .SelectMany(
                                    group => group.custPA.DefaultIfEmpty(),
                                    (group, temp) => new CustomerClassificationModelMoreInfo
                                    {
                                        CustId = group.custClass.CustId,
                                        SportGroupId = group.custClass.SportGroupId,
                                        CategoryId = group.custClass.CategoryId,
                                        CreatedTime = group.custClass.CreatedTime,
                                        TurnoverRM = group.custClass.TurnoverRM,
                                        WinlossRM = group.custClass.WinlossRM,
                                        BetCount = group.custClass.BetCount,
                                        ActiveDays = group.custClass.ActiveDays,
                                        LastXDaysMargin = group.custClass.LastXDaysMargin,
                                        ProbationPeriodWinloss = group.custClass.ProbationPeriodWinloss,
                                        IsGeneralCategory = group.custClass.IsGeneralCategory,
                                        TaggingID = (temp != null) ? temp.TaggingID : 0,
                                        TaggingType = (temp != null) ? temp.TaggingType : 0
                                    });

            return JsonConvert.SerializeObject(result);
        }

        private bool IsInsertMainSuccessful(string errorMess, IEnumerable<CustomerClassificationModelMoreInfo> data)
        {
            if (!string.IsNullOrEmpty(errorMess))
            {
                LogService.Error($"CustomerClassification.AutoTag Service - InsertGeneralClassificationCusts - InsertMainCustomerClassification failed:  {errorMess} with Data: {JsonConvert.SerializeObject(data)}");

                return false;
            }

            return true;
        }
    }
}
