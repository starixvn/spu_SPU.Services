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
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class Service
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly IPushDataToExternalService _pushDataToExternalService;
        private const int _newMemberClassification = 201;

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
                var isWriteLogInfo = AppSettings.IsWriteLogInfo;

                do
                {
                    newClassification = _dataCenterService.GetNewClassification();

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
            var isWriteLogInfo = AppSettings.IsWriteLogInfo;
            var newGeneralClassificationChunk = generalClassification.Chunk(AppSettings.BatchSize).ToList();

            foreach (var listClassification in newGeneralClassificationChunk)
            {
                try
                {
                    //1.1. Check Association With PA
                    var customerAssociationProblemAccounts = _dataCenterService.GetCustomerAssociationProblemAccount(this.GetCustIds(generalClassification));

                    var jsonClassification = MergeCustomerClassificationInfo(listClassification, customerAssociationProblemAccounts);

                    // 1.2. Insert data in CTS
                    var typeCustModel = _dataCenterService.InsertMainCustomerClassification(jsonClassification);

                    if (string.IsNullOrEmpty(typeCustModel.ErrorMessage))
                    {
                        var custIdList = string.Join(",", listClassification.Select(c => c.CustId).Distinct());

                        // 2. Update IsApplied in mainDB
                        _dataCenterService.UpdateAppliedStatusClassification(custIdList, true);

                        // 3. Get lastest classification in CTS
                        var latestCustIdsChunk = typeCustModel.CustIds.Chunk(AppSettings.PushCCToExternalChunkSize);

                        foreach (var custIds in latestCustIdsChunk)
                        {
                            var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",", custIds));

                            if (lastestClassifications != null && lastestClassifications.Any())
                            {
                                // 4. Update CustomerClass in table custinfo to MainDB
                                var isSuccessToMain = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, 2);

                                if (isSuccessToMain)
                                {
                                    var updateApplyCCs = lastestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                                    var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                                    // 5. Update IsAppliedCC = 1 in CTS
                                    _dataCenterService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);
                                }

                                // 6. Push CC to Soon88
                                _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                            }
                        }
                    }
                    else
                    {
                        LogService.Error($"CustomerClassification.AutoTag Service - InsertGeneralClassificationCusts - InsertMainCustomerClassification failed:  {typeCustModel.ErrorMessage} with Data: {jsonClassification}");
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

        private string GetCustIds(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification)
        {
            if (generalClassification != null && generalClassification.Any())
            {
                var custIds = generalClassification.Select(x => x.CustId);

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
                                        TaggingID = (temp != null)? temp.TaggingID : 0,
                                        TaggingType = (temp != null)? temp.TaggingType : 0
                                    });

            return JsonConvert.SerializeObject(result);
        }
    }
}
