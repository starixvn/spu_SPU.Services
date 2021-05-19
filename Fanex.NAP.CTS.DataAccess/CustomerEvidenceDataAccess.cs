namespace Fanex.NAP.CTS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.RemoveEvidenceMultipleAccounts;
    using Fanex.NAP.CTS.Criteria.RemoveMultipleEvidences;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.RemoveEvidenceMultipleAccounts;
    using Fanex.NAP.FS.XML;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerEvidenceDataAccess
    {
        public IEnumerable<CustomerEvidenceFromFileModel> GetCustomerEvidenceFromFile()
        {
            var criteria = new GetCustomerEvidenceFromFileCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerEvidenceFromFileModel>(criteria);
            }
        }

        public ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog)
        {
            var criteria = new RemoveCustEvidencesByCTSCustIdsCriteria
            {
                NewAccountEvidences = jsonData,
                SessionId = sessionId,
                IsRollBack = isRollBack,
                IsClearLog = isClearLog
            };

            var result = new ResultRemoveCustomerEvidence();           
            var repository = new DynamicRepository();

            result.CustEviRemovedList = repository.Fetch<CustomerEvidenceRemovedModel>(criteria);

            if (string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                result.DBResult = new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
            }
            else
            {
                result.DBResult = new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
            }

            return result;
        }

        public DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite)
        {
            var criteria = new InsertCustEvidencesCriteria
            {
                NewAccountEvidences = jsonData,
                UserId = userId,
                IsOverride = isOverwrite
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
        }

        public DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan)
        {
            var criteria = new SyncCustEvidenceToCTSCategoryCriteria
            {
                CustEvidences = jsonData,
                IsRescan = isRescan
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
        }

        public InsertCategoryResultModel SyncCustomerEvidenceToCategory2(string jsonData, bool isRescan)
        {
            var custIds = new List<int>();
            var repository = new DynamicRepository();

            if (string.IsNullOrEmpty(jsonData))
            {
                return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = custIds }; ;
            }

            var criteria = new SyncCustEvidenceToCTSCategoryCriteria
            {
                CustEvidences = jsonData,
                IsRescan = isRescan
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            return new InsertCategoryResultModel() { ErrorMessage = criteria.ErrorMessage, CustIds = (custIds != null) ? custIds : new List<int>() };
        }

        public IEnumerable<CustomerCategoryLastedModel> GetCustomerCategoryLasted(string idList, bool isCustId, bool isExcludedDetailedCategory)
        {
            var criteria = new GetCustomerCategoryLastedCriteria
            {
                IDList = idList,
                IsUsingCustID = isCustId,
                IsExcludedDetailedCategory = isExcludedDetailedCategory
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerCategoryLastedModel>(criteria).ToList();
            }
        }

        public DBResult UpdateProcessedCustEvidence(string idList, bool isProcessed, int userId)
        {
            var criteria = new UpdateProcessedCustEvidenceCriteria
            {
                IdList = idList,
                IsProcessed = isProcessed,
                UserId = userId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
        }

        public IEnumerable<int> GetAssociatedAccountByCTSCustIds(string ctsCustIds)
        {
            var criteria = new GetAssociatedAccountByCTSCustIdsCriteria
            {
                CTSCustIdList = ctsCustIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria);
            }
        }

        public DBResult UpdateBetLimitOfFlaggedCustomer(string custIds)
        {
            var criteria = new UpdateBetLimitByEvidenceAddingCriteria
            {
                CustIDList = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
        }

        public IEnumerable<CustomerEvidenceQueueModel> GetCustomerEvidenceQueue()
        {
            var criteria = new GetCustEvidenceQueueCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerEvidenceQueueModel>(criteria);
            }
        }

        public DBResult ModifyCustomerEvidence(string customerEvidence, int actionId, out string custIdResultList)
        {
            custIdResultList = string.Empty;

            var criteria = new ModifyCustomerEvidenceFromQueueCriteria
            {
                CustomerEvidence = customerEvidence,
                ActionId = actionId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                custIdResultList = criteria.CustIdResultList;

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };

        }

        public DBResult RollbackCustomerEvidence(long sessionId)
        {
            var criteria = new RollbackCustomerEvidenceCriteria
            {
                SessionId = sessionId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
        }

        public DBResult UpdateCustomerEvidence(string processedIdList)
        {
            var criteria = new UpdateCustomerEvidenceCriteria
            {
                ProcessedIdList = processedIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                if (string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                }
            }

            return new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };

        }
    }
}
