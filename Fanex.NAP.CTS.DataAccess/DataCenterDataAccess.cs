namespace Fanex.NAP.CTS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.SPU.Model;
    using Fanex.NAP.FS.XML;
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

        public void UpdateNewClassification(long maxCustId)
        {
            var repository = new DynamicRepository();

            var criteria = new CustomerClassificationUpdateNewClassificationCriteria
            {
                MaxCustId = maxCustId
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

        public void ArchiveTransaction(int numOfTickets)
        {
            var criteria = new ArchiveTransactionAfterTransformingCriteria
            {
                NoOfTickets = numOfTickets
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
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

        public IEnumerable<CustomerClassificationModel> GetCustomerForRealtimeScanning(string custIdList)
        {
            var repository = new DynamicRepository();

            var criteria = new GetCustomerClassificationByCategoryCriteria
            {
                CategoryIdList = "5", // Hard code for VIP Category
                CustIdList = custIdList
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> ClassifyCustomerDaily(IEnumerable<CustomerClassificationModel> customers)
        {
            var repository = new DynamicRepository();

            var criteria = new CategorizeCustomerDailyCriteria
            {
                CustIdList = string.Join(",", customers.Select(c => c.CustId))
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }

        public IEnumerable<CustomerClassificationModel> ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            var repository = new DynamicRepository();

            var criteria = new CategorizeCustomerRealtimeCriteria
            {
                VIPCustomersXML = customers.ToXmlString()
            };

            return repository.Fetch<CustomerClassificationModel>(criteria);
        }
    }
}
