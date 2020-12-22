namespace Fanex.NAP.CTS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.MainDB;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.FS.XML;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public List<int> TransformCustomerFromMainDB(string customerJson)
        {
            var criteria = new TransformCustomerFromMainDbCriteria
            {
                CustomerJson = customerJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria).ToList();
            }
        }

        public List<int> InsertCategoryForNewCustomer(string custIds)
        {
            var criteria = new InsertNewCustomerCategoryCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria).ToList();
            }
        }

        public int FillUpCustomerFromMainDB(string customerJson)
        {
            var criteria = new TransformCustomerFromMainDbCriteria
            {
                CustomerJson = customerJson
            };

            return repository.Execute(criteria);
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

        public int InsertAssociation(string associationJson)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertAssociationCriteria
            {
                AssociationJson = associationJson
            };

            return repository.Execute(criteria);
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

        public List<CTSCustomer> GetCustomersForUpdateUsername2(int rowSize, bool isUpdateCustSub, int maxCTSCustId)
        {
            var criteria = new GetCustomersForUpdateUsername2Criteria
            {
                Size = rowSize,
                MaxCustId = maxCTSCustId,
                IsUpdateCustSub = isUpdateCustSub
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CTSCustomer>(criteria).ToList();
            }
        }

        public List<CustomerMainDbModel> GetUsername2FromMainDb(string custIds, bool isCustSub)
        {
            var criteria = new GetUsername2FromMainDb
            {
                CustIds = custIds,
                IsGetCustSub = isCustSub
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerMainDbModel>(criteria).ToList();
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

            var criteria = new InsertCustomerGeneralCategoryToMainDBCriteria
            {
                custList = custIdList.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }

        public void UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<CustomerClassModel> custClassList)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertCustomerGeneralCategoryToMainDBCriteria
            {
                custList = custClassList.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void InsertProblemAccountCategoryToMainDB(List<ProblemAccountForMainDBModel> custIdList)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertCustomerGeneralCategoryToMainDBCriteria
            {
                custList = custIdList.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void InsertProblemCustomerCategoryToMainDB(List<ProblemCustomerForMainDBModel> custIdList)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertCustomerGeneralCategoryToMainDBCriteria
            {
                custList = custIdList.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void AutoTagCustomerClassification(string newClassification)
        {
            var repository = new DynamicRepository();

            var criteria = new AutoTagCustomerClassificationCriteria
            {
                NewClassification = newClassification
            };

            repository.Execute(criteria);
        }

        public InsertCategoryResultModel InsertMainCustomerClassification(string newClassification)
        {
            var custIds = new List<int>();

            var repository = new DynamicRepository();

            var criteria = new InsertMainCustomerClassificationCriteria
            {
                NewClassification = newClassification
            };

            custIds = repository.Fetch<int>(criteria).ToList();

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

        public IEnumerable<CustomerClassificationModel> GetCustomerForDailyScanning(int batchQuantity, int rowQuantityPerBatch, int parentCateId)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationForScanningCriteria()
            {
                BatchQuantity = batchQuantity,
                RecordQuantityPerBatch = rowQuantityPerBatch,
                ParentCategoryId = parentCateId
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

        public IEnumerable<CustomerClassificationModel> GetCustomerForRealtimeScanning(string custSportJson)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationByCategoryCriteria
            {
                CategoryIdList = "2,5", // Hard code for New Cust & VIP Category
                CustSportsJson = custSportJson
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryForRunRealtime(string custSportJson)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationByCategoryCriteria
            {
                CategoryIdList = "201,202,205", // Hard code for 'New member' & 'Main Normal Account' & 'Main VIP'
                CustSportsJson = custSportJson
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public void ClassifyCustomerDaily(IEnumerable<CustomerClassificationModel> customers)
        {
            var repository = new DynamicRepository();

            var criteria = new CategorizeCustomerDailyCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
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

        public void ClassifyCustomerDailyDone(string custSportJson, int parentCateId)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationForScanningCompleteCriteria
            {
                CustSportJson = custSportJson,
                ParentCategoryId = parentCateId
            };

            repository.Execute(criteria);
        }

        public void ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            var repository = new DynamicRepository();

            var criteria = new CategorizeCustomerRealtimeCriteria
            {
                CustomersXML = customers.ToXmlString()
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

        public InsertCategoryResultModel InsertProblemAccount(string problemAccountJson)
        {
            var custIds = new List<int>();
            var repository = new DynamicRepository();

            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = problemAccountJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            return new InsertCategoryResultModel() { ErrorMessage = criteria.ErrorMessage, CustIds = custIds };
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

        public IEnumerable<int> GetAllSpecialCustomerClass()
        {
            var repository = new DynamicRepository();

            var criteria = new GetAllSpecialCustomerClassCriteria
            {
                IsGetAllInfo = false
            };

            return repository.Fetch<int>(criteria);
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

        public InsertRobotUserResultModel InsertRobotUsers(string custIds)
        {
            var custClassification = new List<LatestClassificationModel>();
            var repository = new DynamicRepository();

            var criteria = new InsertRobotUsersIntoSpecialCustClassCriteria
            {
                CustIds = custIds
            };

            custClassification =  repository.Fetch<LatestClassificationModel>(criteria).ToList();

            return new InsertRobotUserResultModel() { ErrorMessage = criteria.ErrorMessage, CustClassification = custClassification };
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

        public IEnumerable<ProblemAccountForMainDBModel> GetLastestCategory(string ctsCustID, bool isCustId, bool isExcludedDetailedCategory)
        {
            var criteria = new GetLastestCategoryCriteria()
            {
                CustIDList = ctsCustID,
                IsUsingCustID = isCustId,
                IsExcludedDetailedCategory = isExcludedDetailedCategory

            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountForMainDBModel>(criteria).ToList();
            }
        }

        public IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
        {
            var criteria = new GetLastestClassificationCriteria()
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LatestClassificationModel>(criteria);
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

                return new DBResult {
                    ErrorCode = !string.IsNullOrEmpty(criteria.ErrorMessage) ? - 1 : 0,
                    ErrorMsg = criteria.ErrorMessage
                };                
            }
        }
    }
}
