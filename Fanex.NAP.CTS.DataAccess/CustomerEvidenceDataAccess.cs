namespace Fanex.NAP.CTS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Model;
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

        public DBResult RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog)
        {
            var criteria = new RemoveCustEvidencesByCTSCustIdsCriteria
            {
                NewAccountEvidences = jsonData,
                SessionId = sessionId,
                IsRollBack = isRollBack,
                IsClearLog = isClearLog
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

        public DBResult SyncCustomerEvidenceToCategory(string jsonData)
        {
            var criteria = new SyncCustEvidenceToCTSCategoryCriteria
            {
                CustEvidences = jsonData
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

        public DBResult SyncCustomerClassToMainDb(IEnumerable<SyncCustomerClassToMainModel> ccList)
        {
            var repository = new DynamicRepository();

            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = ccList.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }

        public DBResult UpdateProcessedCustEvidence(string jsonData, int userId)
        {
            var criteria = new UpdateProcessedCustEvidenceCriteria
            {
                IdList = jsonData,
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
    }
}
