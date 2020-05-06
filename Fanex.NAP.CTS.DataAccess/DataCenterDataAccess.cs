namespace Fanex.NAP.CTS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.SPU.Model;
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

        public void TransformAssociationAffectedEvidence(int batchSize)
        {
            var repository = new DynamicRepository();

            var criteria = new TransformAssociationAffectedEvidenceCriteria
            {
                BatchSize = batchSize
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

        public List<CustomerClassificationModel> GetNewClassification()
        {
            var criteria = new CustomerClassificationGetNewClassificationCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerClassificationModel>(criteria).ToList();
            }
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
    }
}
