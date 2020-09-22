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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataCenterDataAccess
    {
        public void TransformDCSAccount(string accountJson)
        {
            var repository = new DynamicRepository();

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

        public List<int> FillUpCustomerFromMainDB(string customerJson)
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
            var repository = new DynamicRepository();

            var criteria = new TransformAssociationAffectedEvidenceCriteria
            {
                BatchSize = batchSize
            };

            return repository.Fetch<int>(criteria);
        }

        public void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateBetLimitByEvidenceRemovingCriteria
            {
                CustIDList = string.Join(",", custIds)
            };

            repository.Execute(criteria);
        }


        public void TransformUpdatingLastLoginTime(int batchSize)
        {
            var repository = new DynamicRepository();

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

        public void UpdateCustomerGeneralClassificationtoMainDB(List<CustomerGeneralClassificationModel> custIdList)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertCustomerGeneralCategoryToMainDBCriteria
            {
                custList = custIdList.ToXmlString()
            };

            repository.Execute(criteria);
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

        public void InsertProblemAccountCategoryToMainDB(List<ProblemAccountsModel> custIdList)
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

        public IEnumerable<CustomerGeneralClassificationModel> InsertMainCustomerClassification(string newClassification)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertMainCustomerClassificationCriteria
            {
                NewClassification = newClassification
            };

            return repository.Fetch<CustomerGeneralClassificationModel>(criteria);
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
                CategoryIdList = "202,205", // Hard code for 'Main Normal Account' & 'Main VIP'
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

        public void ClassifyCustomerGeneralCategoryDaily(IEnumerable<CustomerClassificationModel> customers)
        {
            var repository = new DynamicRepository();

            var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
            {
                ListCustIds = string.Join(",", customers.Select(c => c.CustId))
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

        public IEnumerable<ProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime dateTime)
        {
            var criteria = new GetProblemAccountsCriteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountsModel>(criteria).ToList();
            }
        }

        public IEnumerable<ProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime dateTime)
        {
            var criteria = new GetProblemAccountsCN88Criteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemAccountsModel>(criteria).ToList();
            }
        }

        public void InsertProblemAccount(string problemAccountJson)
        {
            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = problemAccountJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
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

        public IEnumerable<CustomerClassModel> InsertRobotUsers(string custIds)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertRobotUsersIntoSpecialCustClassCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<CustomerClassModel>(criteria);
        }
    }
}
