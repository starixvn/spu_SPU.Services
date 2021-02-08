using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Model;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.DCS.Services.Interface
{
    public interface IDataCenterService
    {
        List<int> GetUpdatedAccount(int size);

        DataResult UpdateAccountId(string accountIdJson);

        List<DevicePackage> GetStagingTransIdForDevice(int size, int noOfBatch);

        DataResult InsertDevice(List<DevicePackage> transId);

        List<RawAccountModel> GetRawAccount(int numOfAccount, int numOfBatch);

        List<RawAccountModel> GetRawUnsuccessAccount(int numOfAccount);

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        List<CustomerMainDbModel> GetCustomerFromMainDB(int numOfAccount, Int64 lastScannedCustID);

        List<CustomerMainDbModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID);

        int GetCurrentMaxCustId(bool isCustSub);

        List<RawAssociationModel> GetRawAssociation(int numOfAssociation, int numOfBatch);

        List<RawAssociationModel> GetRawUnsuccessAssociation(int numOfAssociation);

        List<RawTransactionByIpModel> GetRawTransByIp(int numOfTrans, int numOfBatch);

        List<RawTransactionByIpModel> GetUnsuccessTransByIp(int numOfTrans, int numOfBatch);

        void ArchiveTransaction07(int size);

        LastScannedCustModel GetLastInsertCust ();

        LastUpdateValueModel GetLastUpdatedTime();
    }
}
