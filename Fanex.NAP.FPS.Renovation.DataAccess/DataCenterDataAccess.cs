namespace Fanex.NAP.FPS.Renovation.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.FPS.Renovation.Criteria;
    using Fanex.NAP.FPS.Renovation.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataCenterDataAccess
    {       
        public List<NewCustomerModel> GetNewCustomer(int size)
        {           
            var criteria = new GetNewCustomerCriteria
            {
                Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<NewCustomerModel>(criteria).ToList();
            }
        }

        public DataResult UpdateCustomerInfo(string customerInfo)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateCustomerInfoCriteria
            {
                CustomerInfo = customerInfo
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public List<Model.CustomerInfoModel> GetCustomerInfo(string newCustomers)
        {
            var criteria = new GetCustomerInfoCriteria
            {
                CustomerList = newCustomers
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerInfoModel>(criteria).ToList();
            }
        }

        public List<Model.AssociationModel> GetAssociation(int isSysnGraphDB, int size)
        {

            var criteria = new GetAssociationForGraphDBCriteria
            {
                SyncGraphDB = isSysnGraphDB,
                Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AssociationModel>(criteria).ToList();
            }
        }

        public DataResult UpdateIsSyncGraphDBByAssociationId(string association)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateIsSyncGraphDBByAssociationIdCriteria
            {
                Association = association
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public List<AccountExistedModel> AccountProcess(int fromId, int toId)
        {
            var criteria = new ProcessAccountCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AccountExistedModel>(criteria).ToList();
            }
        }

        public void InsertRawAccount(string jsonAccounts)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertRawAccountCriteria
            {
                AccountJson = jsonAccounts
            };

            repository.Execute(criteria);           
        }

        public void UserAgentProcess(int fromId, int toId)
        {
            var repository = new DynamicRepository();

            var criteria = new ProcessUserAgentCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);          
        }

        public void TransactionProcess(int fromId, int toId)
        {
            var repository = new DynamicRepository();

            var criteria = new ProcessTransactionCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);           
        }

        public DataResult CompleteProcess(int fromId, int toId)
        {
            var repository = new DynamicRepository();

            var criteria = new ProcessCompletedCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }       

        public DataResult DeviceProcess(int fromId, int toId)
        {
            var repository = new DynamicRepository();

            var criteria = new ProcessDeviceCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public DataResult UnhandleSubscriberActionProcess(int fromId, int toId)
        {
            var repository = new DynamicRepository();

            var criteria = new ProcessUnhandleSubscriberActionCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public List<UserAgentModel> GetUserAgent(int size)
        {
            var criteria = new GetUserAgentCriteria
            {
               Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<UserAgentModel>(criteria).ToList();
            }
        }                
                
        public DataResult UpdateUserAgent(string userAgentJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateUserAgentCriteria
            {
                UserAgentJson = userAgentJson
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public List<int> GetUpdatedAccountList(int size)
        {
            var criteria = new GetUpdatedAccountListCriteria
            {
                Size = size
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria).ToList();
            }
        }

        public DataResult UpdateAccountId(string accountIdJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateAccountIdCriteria
            {
                AccountIdJson = accountIdJson
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public List<DevicePackage> GetStagingTransIdForDevice(int size, int noOfBatch)
        {
            var criteria = new GetUpdatedDeviceListCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DevicePackage>(criteria).ToList();
            }
        }

        public DataResult InsertDevice(string transId)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateDeviceIdCriteria
            {
                TransId = transId
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }

        public DataResult InsertFingerPrint(int fromId, int toId)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertFingerPrintCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }
}
