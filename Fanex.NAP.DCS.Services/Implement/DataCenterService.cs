using Fanex.NAP.DCS.DataAccess;
using Fanex.NAP.DCS.Model;
using Fanex.NAP.DCS.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.DCS.Services.Implement
{
    public class DataCenterService : IDataCenterService
    {
        private readonly DataAccess.DataCenterDataAccess dataCenter;
        
        public DataCenterService()
        {
            dataCenter = new DataCenterDataAccess();
        }

        public List<int> GetUpdatedAccount(int size)
        {
            return dataCenter.GetUpdatedAccountList(size);
        }

        public List<DevicePackage> GetStagingTransIdForDevice(int size, int noOfBatch)
        {
            return dataCenter.GetStagingTransIdForDevice(size, noOfBatch);
        }

        public DataResult UpdateAccountId(string accountIdJson)
        {
            return dataCenter.UpdateAccountId(accountIdJson);
        }

        public DataResult InsertDevice(List<DevicePackage> transId)
        {
            var transJson = JsonConvert.SerializeObject(transId);

            return dataCenter.InsertDevice(transJson);
        }

        public List<RawAccountModel> GetRawAccount(int numOfAccount, int numOfBatch)
        {
            return dataCenter.GetRawAccount(numOfAccount, numOfBatch);
        }

        public UpdateCustomerModel GetCustomerFromMainDB(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataCenter.GetCustomerFromMainDB(numOfAccount, lastScannedCustID);
        }

        public List<CustomerMainDBModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataCenter.GetCustSubFromMainDB(numOfAccount, lastScannedCustID);
        }

        public int GetCurrentMaxCustId(bool isCustSub)
        {
            return dataCenter.GetMaxCustIdInCTSCustomer(isCustSub);
        }

        public List<RawAssociationModel> GetRawAssociation(int numOfAssociation, int numOfBatch)
        {
            return dataCenter.GetRawAssociation(numOfAssociation, numOfBatch);
        }


        public List<RawTransactionByIpModel> GetRawTransByIp(int numOfTrans, int numOfBatch)
        {
            return dataCenter.GetRawTransactionByIp(numOfTrans, numOfBatch);
        }

        public List<RawAccountModel> GetRawUnsuccessAccount(int numOfAccount)
        {
            return dataCenter.GetRawUnsuccessAccount(numOfAccount);
        }

        public List<RawTransactionByIpModel> GetUnsuccessTransByIp(int numOfTrans, int numOfBatch)
        {
            return dataCenter.GetUnsuccessRawTransactionByIp(numOfTrans, numOfBatch);
        }

        public List<RawAssociationModel> GetRawUnsuccessAssociation(int numOfAssociation)
        {
            return dataCenter.GetRawUnsuccessAssociation(numOfAssociation);
        }

        public void ArchiveTransaction07(int size)
        {
            dataCenter.ArchiveTransaction07(size); 
        }

        public IEnumerable<DateTime> GetScheduleForRunArchiveTrans07()
        {
            return dataCenter.GetArchiveTransaction07Schedule();
        }

        public void MoveDataArchiveTransaction07(DateTime runningDate, int batchSize, ref bool isCompleted)
        {
            dataCenter.MoveDataArchiveTransaction07(runningDate, batchSize, ref isCompleted);
        }

        public void DeleteDataArchiveTransaction07(DateTime runningDate, ref bool isCompleted)
        {
            dataCenter.DeleteDataArchiveTransaction07(runningDate, ref isCompleted);
        }

    }
}
