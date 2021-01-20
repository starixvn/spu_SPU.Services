namespace Fanex.NAP.DCS.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.DCS.Criteria;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.SPU.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataCenterDataAccess
    {
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
                FromTransId = fromId,
                ToTransId = toId
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

        public List<RawAccountModel> GetRawAccount(int quantityPerBatch, int numOfBatch)
        {
            var criteria = new GetRawAccountListCriteria
            {
                NumberOfAccount = quantityPerBatch,
                NumberOfBatch = numOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawAccountModel>(criteria).ToList();
            }
        }

        public List<RawAccountModel> GetRawUnsuccessAccount(int quantityPerBatch)
        {
            var criteria = new GetRawUnsucessAccountListCriteria
            {
                NumberOfAccount = quantityPerBatch                
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawAccountModel>(criteria).ToList();
            }
        }

        public List<CustomerMainDbModel> GetCustomerFromMainDB(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustomersFromMainDbCriteria
            {
                LastCustId = lastScannedCustID,
                RowSize = quantityPerBatch,
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerMainDbModel>(criteria).ToList();
            }
        }

        public List<CustomerMainDbModel> GetCustSubFromMainDB(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustSubFromMainDbCriteria
            {
                LastCustSubId = lastScannedCustID,
                RowSize = quantityPerBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerMainDbModel>(criteria).ToList();
            }
        }

        public int GetMaxCustIdInCTSCustomer(bool isCustSub)
        {
            var criteria = new GetMaxCustIdCriteria();
            criteria.IsCustSub = isCustSub;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria).FirstOrDefault();
            }
        }

        public List<RawAssociationModel> GetRawAssociation(int quantityPerBatch, int numOfBatch)
        {
            var criteria = new GetRawAssociationListCriteria
            {
                NumberOfAssociation = quantityPerBatch,
                NumberOfBatch = numOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawAssociationModel>(criteria).ToList();
            }
        }

        public List<RawAssociationModel> GetRawUnsuccessAssociation(int quantityPerBatch)
        {
            var criteria = new GetRawUnsucessAssociationListCriteria
            {
                NumberOfAssociation = quantityPerBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawAssociationModel>(criteria).ToList();
            }
        }

        public List<RawTransactionByIpModel> GetRawTransactionByIp(int quantityPerBatch, int numOfBatch)
        {
            var criteria = new GetRawTransactionByIpCriteria
            {
                NumberOfTrans = quantityPerBatch,
                NumberOfBatch = numOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawTransactionByIpModel>(criteria).ToList();
            }
        }

        public List<RawTransactionByIpModel> GetUnsuccessRawTransactionByIp(int quantityPerBatch, int numOfBatch) 
        {
            var criteria = new GetRawUnsuccessTransactionIPListCriteria
            {
                NumberOfTrans = quantityPerBatch,
                NumberOfBatch = numOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawTransactionByIpModel>(criteria).ToList();
            }
        }

        public void ArchiveTransaction07(int tranSize)
        {
            var repository = new DynamicRepository();

            var criteria = new ArchiveTransaction07Criteria
            {
                TranSize = tranSize
            };

            repository.Execute(criteria);
        }

        public IEnumerable<DateTime> GetArchiveTransaction07Schedule()
        {
            var repository = new DynamicRepository();

            var criteria = new GetScheduleForArchiveTransaction07Criteria();

            return repository.Fetch<DateTime>(criteria);
        }

        public void MoveDataArchiveTransaction07(DateTime runningDate, int batchSize, ref bool isComplete)
        {
            var repository = new DynamicRepository();

            var criteria = new MoveDataForArchiveTransaction07Criteria
            {
                ArchivedDate = runningDate,
                BatchSize = batchSize
            };

            repository.Execute(criteria);
            isComplete = criteria.IsCompleted;
        }

        public void DeleteDataArchiveTransaction07(DateTime runningDate, ref bool isComplete)
        {
            var repository = new DynamicRepository();

            var criteria = new DeleteDataForArchiveTransaction07Criteria
            {
                ArchivedDate = runningDate,
            };

            repository.Execute(criteria);
            isComplete = criteria.IsCompleted;
        }

        public LastScannedCustModel GetLastInsertCust()
        {
            var criteria = new GetLastInsertCustCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LastScannedCustModel>(criteria).FirstOrDefault();
            }
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            var criteria = new GetLastUpdatedTimeCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LastUpdateValueModel>(criteria).FirstOrDefault();
            }
        }

        public TranformCustModel GetUpdatedCustFromCustHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromCustHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<TranformCustModel>(criteria).FirstOrDefault();
            }
        }
    }
}
