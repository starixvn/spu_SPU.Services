namespace Fanex.NAP.CTS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.Association;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.MainDB;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.FS.XML;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.Core.Constants;

    public class DataCenterDataAccess
    {
        readonly IDynamicRepository repository;

        public DataCenterDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void TransformDCSAccount(string accountJson)
        {
            var criteria = new TransformDCSAccountToCTSCustomerCriteria
            {
                AccountJson = accountJson
            };

            repository.Execute(criteria);
        }

        public IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(string customerJson)
        {
            var custInfos = new List<InsertCustomerModel>();

            var criteria = new TransformCustomerFromMainDbCriteria
            {
                CustomerJson = customerJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custInfos = db.Query<InsertCustomerModel>(criteria).ToList();
            }

            return custInfos;
        }

        public void TransformAssociationByDevice(string associationJson)
        {
            var criteria = new TransformAssociationByDeviceCriteria
            {
                AssociationList = associationJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        public void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIPJson)
        {
            var repository = new DynamicRepository();

            var criteria = new TransformDCSTransactionIPToCTSAssociationByIPCriteria
            {
                TransactionJson = transactionByIPJson
            };

            repository.Execute(criteria);
        }

        public List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId)
        {
            var criteria = new GetCustomersWithNullUsername2Criteria
            {
                Size = rowSize,
                PreCTSCustID = preCTSCustId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustIdModel>(criteria).ToList();
            }
        }

        public List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs)
        {
            var criteria = new GetCustomersWithHavingUsername2Criteria
            {
                cust = custs.ToXmlString()
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustIdModel>(criteria).ToList();
            }
        }

         public int UpdateUsername2(string customerJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateUsername2Criteria
            {
                CustomerJson = customerJson
            };

            return repository.Execute(criteria);
        }

        public IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize)
        {
            var criteria = new TransformAssociationAffectedEvidenceCriteria
            {
                BatchSize = batchSize
            };

            return repository.Fetch<int>(criteria);
        }

        public void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds)
        {
            var criteria = new UpdateBetLimitByEvidenceRemovingCriteria
            {
                CustIDList = string.Join(",", custIds)
            };

            repository.Execute(criteria);
        }


        public void TransformUpdatingLastLoginTime(int batchSize)
        {
            var criteria = new TransformUpdatingLastLoginTimeCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }

        public List<CustomerClassificationModelMoreInfo> GetNewClassification()
        {
            var criteria = new CustomerClassificationGetNewClassificationCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerClassificationModelMoreInfo>(criteria).ToList();
            }
        }

        public void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory)
        {
            var repository = new DynamicRepository();

            var criteria = new CustomerClassificationUpdateNewClassificationCriteria
            {
                ListCustIds = custIdList,
                IsGeneralCategory = isGeneralCategory
            };

            repository.Execute(criteria);
        }

        public DBResult UpdateCustomerGeneralClassificationtoMainDB(List<LatestClassificationModel> custIdList)
        {
            var repository = new DynamicRepository();

            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = custIdList.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }

        public void UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<CustomerClassModel> custClassList)
        {
            var repository = new DynamicRepository();

            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = custClassList.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelModel> dangerLevel, DANGERLEVEL typeId)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateDangerLevelCriteria
            {
                CustList = dangerLevel.ToXmlString(),
                DangerType = (int)typeId
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }

        public InsertCategoryResultModel InsertMainCustomerClassification(string newClassification)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertMainCustomerClassificationCriteria
            {
                NewClassification = newClassification
            };

            var custIds = repository.Fetch<int>(criteria).ToList();

            return new InsertCategoryResultModel() { ErrorMessage = criteria.ErrorMessage, CustIds = custIds };
        }


        public IEnumerable<CustomerCategoryClassificationModel> GetCustomerClassByCustId(string custIdList)
        {
            var criteria = new GetCustomerClassByCustIdCriteria
            {
                CustIds = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerCategoryClassificationModel>(criteria);
            }
        }

        public void DCSMoveTransToTransaction07(string transIdList)
        {
            var criteria = new MoveToTransaction07Criteria
            {
                TransIdList = transIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        public IEnumerable<TransactionModel> DCSMoveTransToTransaction07GetPackage(int batchSize, int NoOfBatch)
        {
            var repository = new DynamicRepository();
            var criteria = new MoveToTransaction07GetPackageCriteria
            {
                BatchSize = batchSize,
                NumberOfBatch = NoOfBatch
            };

            return repository.Fetch<TransactionModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerForDailyScanning(int batchQuantity, int rowQuantityPerBatch)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationForScanningCriteria()
            {
                BatchQuantity = batchQuantity,
                RecordQuantityPerBatch = rowQuantityPerBatch
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerHasChangeForRunRealtime()
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerHasChangeForRunRealtimeCriteria();

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerGeneralCategoryHasChangeForRunRealtimeCriteria();

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryForRunRealtime(string custSportJson, string categoryIds)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationByCategoryCriteria
            {
                CategoryIdList = categoryIds,
                CustSportsJson = custSportJson
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public void ClassifyCustomerGeneralCategoryDaily(string customers, bool isCheckLast14Days = true)
        {
            var repository = new DynamicRepository();

            var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
            {
                ListCustIds = customers,
                IsCheckLast14Days = isCheckLast14Days
            };

            repository.Execute(criteria);
        }

        public void ClassifyCustomerDailyDone(string custSportJson)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationForScanningCompleteCriteria
            {
                CustSportJson = custSportJson
            };

            repository.Execute(criteria);
        }

        public void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            var repository = new DynamicRepository();

            var criteria = new CategorizeCustomerGeneralCategoryRealtimeCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            var repository = new DynamicRepository();

            var criteria = new RealtimeUpdateLastScanTimeCriteria()
            {
                IsGeneralCategory = isGeneralCategory
            };

            repository.Execute(criteria);
        }

        public IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime? dateTime)
        {
            var criteria = new GetProblemAccountsCriteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MultipleProblemAccountsModel>(criteria).ToList();
            }
        }

        public IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime)
        {
            var criteria = new GetProblemAccountsCN88Criteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MultipleProblemAccountsModel>(criteria).ToList();
            }
        }

        public InsertCategoryResultModel InsertProblemAccount(string problemAccountJson, bool isRescan)
        {
            var custIds = new List<int>();
            var repository = new DynamicRepository();

            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsRescan = isRescan
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            return new InsertCategoryResultModel() { ErrorMessage = criteria.ErrorMessage, CustIds = custIds ?? new List<int>() };
        }

        public IEnumerable<SystemParemeterModel> GetPreviousTimingExecution(int paramId)
        {
            var criteria = new GetPreviousTimingExecutionCriteria
            {
                ParamID = paramId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SystemParemeterModel>(criteria).ToList();
            }
        }

        public void UpdateTimingExecution(string newDataTime, int paramId)
        {
            var criteria = new UpdateTimingExecutionCriteria
            {
                ParamID = paramId,
                ParamValue = newDataTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        public void InsertUserPermission(string permissionUserIdsJson)
        {
            var criteria = new InsertUserPermissionCriteria
            {
                UserIDList = permissionUserIdsJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        public IEnumerable<RobotUser> GetRobotUsers(DateTime fromDate, string creditSiteList)
        {
            var repository = new DynamicRepository();

            var criteria = new GetRobotUserCriteria
            {
                FromDate = fromDate,
                CreditSite = creditSiteList
            };

            return repository.Fetch<RobotUser>(criteria);
        }

        public IEnumerable<ProbationModel> GetProbationCustForScanning(int numberOfRecord)
        {
            var repository = new DynamicRepository();

            var criteria = new GetProbationCustForScanningCriteria
            {
                NumberOfRecord = numberOfRecord
            };

            return repository.Fetch<ProbationModel>(criteria);
        }

        public void UpdateLastScanDateForProbationCust(IEnumerable<ProbationModel> custList)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateLastScanDateForProbationCustCriteria
            {
                CustSportJson = JsonConvert.SerializeObject(custList)
            };

            repository.Execute(criteria);
        }

        public IEnumerable<ProbationModel> ClassifyCategoryForProbationCust(IEnumerable<ProbationModel> custList)
        {
            var repository = new DynamicRepository();

            var criteria = new ClassifyCategoryForProbationCustCriteria
            {
                CustomersXML = custList.ToXmlString()
            };

            return repository.Fetch<ProbationModel>(criteria);
        }

        public void InsertProbationNotification(IEnumerable<ProbationModel> scanList, IEnumerable<ProbationModel> resultList)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertProbationNotificationCriteria
            {
                TotalScanned = scanList.Count(),
                TotalFailed = resultList.Count(),
                TotalGeneralFailed = resultList.Where(c => c.SportGroupId == 200).Count()
            };

            repository.Execute(criteria);
        }

        public IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
        {
            if (string.IsNullOrEmpty(custIds))
            {
                return new List<LatestClassificationModel>();
            }

            var criteria = new GetLastestClassificationCriteria()
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LatestClassificationModel>(criteria);
            }
        }

        public IEnumerable<DangerLevelModel> GetDangerLevel(string custIds, DANGERLEVEL typeId)
        {
            if (string.IsNullOrEmpty(custIds))
            {
                return new List<DangerLevelModel>();
            }

            var criteria = new GetDangerLevelCriteria()
            {
                CustIds = custIds,
                TypeID = (int)typeId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DangerLevelModel>(criteria);
            }
        }

        public IEnumerable<AssociationWithCategory> FeedDataToToSoon88(IEnumerable<CustomerClassModel> custClasses, bool isDelete)
        {
            var repository = new DynamicRepository();

            var criteria = new GetAssociationWithCategoryCriteria()
            {
                CustIds = string.Join(",", custClasses.Select(c => c.CustID).Distinct()),
                IsDelete = isDelete
            };

            return repository.Fetch<AssociationWithCategory>(criteria);
        }

        public DBResult UpdateAppliedCustomerClassification(string problemCustomerJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateAppliedCustomerClassificationCriteria
            {
                ProblemCustomerJson = problemCustomerJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                repository.Fetch<string>(criteria);

                return new DBResult
                {
                    ErrorCode = !string.IsNullOrEmpty(criteria.ErrorMessage) ? -1 : 0,
                    ErrorMsg = criteria.ErrorMessage
                };
            }
        }

        public void UpdateAppliedDangerLevel(string custsJson, DANGERLEVEL typeID)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateAppliedDangerLevelCriteria
            {
                CustJson = custsJson,
                TypeID = (int)typeID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                repository.Execute(criteria);
            }
        }

        public IEnumerable<CustEvidenceModel> GetExistedEvidences(string custIDs)
        {
            var criteria = new GetExistedEvidencesCriteria
            {
                CustIds = custIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustEvidenceModel>(criteria).ToList();
            }
        }

        public IEnumerable<CustomerAssociationProblemAccountModel> GetCustomerAssociationProblemAccount(string custIdList)
        {
            var criteria = new GetAssociationProblemAccountForCustomerClassificationCriteria
            {
                CustIdList = custIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<CustomerAssociationProblemAccountModel>(criteria);

                return result;
            }
        }

        public void ReTranformAccount(string jsonData, long? lastCustID)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            var repository = new DynamicRepository();

            var criteria = new ReTransformAccountCriteria
            {
                CustomerInfo = jsonData,
                LastInsertCustID = lastCustID
            };

            repository.Execute(criteria);
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromMainDB(string custIds, UPDATECUSTOMER sourceType)
        {
            var custInfos = new List<CustomerMainDBModel>();

            var criteria = new GetCustomerInfoFromMainDBCriteria
            {
                CustIds = custIds,
                SourceType = (int)sourceType
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custInfos = db.Query<CustomerMainDBModel>(criteria).ToList();
            }

            return custInfos;
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromCTS(string custIds, UPDATECUSTOMER sourceType)
        {
            var custInfos = new List<CustomerMainDBModel>();

            var criteria = new GetCustomerInfoFromCTSCriteria
            {
                CustIds = custIds,
                SourceType = (int)sourceType
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custInfos = db.Query<CustomerMainDBModel>(criteria).ToList();
            }

            return custInfos;
        }

        public void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateCustInfoCompleteCriteria
            {
                CustInfo = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            repository.Execute(criteria);
        }

        public void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateCustomerCompleteCriteria
            {
                CustomerInfo = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            repository.Execute(criteria);
        }

        public void UpdateCustomerStatusComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateCustomerStatusCompleteCriteria
            {
                CustProductStatus = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            repository.Execute(criteria);
        }

        public void InserAssociationDevice(int batchSize)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertAssociationDeviceCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }

        public IEnumerable<RoleCustomerModel> GetRoleCustomer(string custIds)
        {
            var roleCus = new List<RoleCustomerModel>();

            var criteria = new GetRoleCustomerCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                roleCus = db.Query<RoleCustomerModel>(criteria).ToList();
            }

            return roleCus;
        }

        public IEnumerable<int> GetCustByRobotSiteRule(string custIds)
        {
            var roleCus = new List<int>();

            var criteria = new GetCustByRobotSiteRuleCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                roleCus = db.Query<int>(criteria).ToList();
            }

            return roleCus;
        }

        public string GetExistedRobotUser(string custIds)
        {
            var criteria = new GetExistedRobotUserCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }

            return criteria.RobotList;
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            var criteria = new GetLastUpdatedTimeCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var LastUpdateTimeCustInfo = result.Read<DateTime>().FirstOrDefault();
                    var LastUpdateCustIDCustInfo = result.Read<Int64>().FirstOrDefault();
                    var LastUpdateTimeCustomer = result.Read<DateTime>().FirstOrDefault();
                    var LastUpdateCustIDCustomer = result.Read<Int64>().FirstOrDefault();
                    var LastUpdateTimeCustomerStatus = result.Read<DateTime>().FirstOrDefault();
                    var LastUpdateCustIDCustomerStatus = result.Read<Int64>().FirstOrDefault();

                    return new LastUpdateValueModel()
                    {
                        LastUpdateCustIDCustInfo = LastUpdateCustIDCustInfo,
                        LastUpdateTimeCustInfo = LastUpdateTimeCustInfo,
                        LastUpdateCustIDCustomer = LastUpdateCustIDCustomer,
                        LastUpdateTimeCustomer = LastUpdateTimeCustomer,
                        LastUpdateCustIDCustProductStatus = LastUpdateCustIDCustomerStatus,
                        LastUpdateTimeCustProductStatus = LastUpdateTimeCustomerStatus
                    };
                }
            }

        }

        public TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromCustInfoHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var repository = new DynamicRepository();

            var output = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel()
            {
                LastUpdatedTime = output.firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = output.firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = output.secondResultSet.SingleOrDefault()
            };
        }

        public TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromCustomerHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var repository = new DynamicRepository();

            var output = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel()
            {
                LastUpdatedTime = output.firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = output.firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = output.secondResultSet.SingleOrDefault()
            };
        }

        public TranformCustModel GetUpdatedCustFromProductStatus(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromProductStatusHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var repository = new DynamicRepository();

            var output = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel()
            {
                LastUpdatedTime = output.firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = output.firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = output.secondResultSet.SingleOrDefault()
            };
        }
    }
}
