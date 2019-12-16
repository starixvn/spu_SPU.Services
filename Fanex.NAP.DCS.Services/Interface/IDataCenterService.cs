using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Model;
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

        List<RawAccountModel> GetRawUnsuccessAccount(int numOfAccount, int numOfBatch);

        List<CustomerMainDbModel> GetCustomerFromMainDB(int numOfAccount, int numOfBatch, int maxCustId);

        List<CustomerMainDbModel> GetCustSubFromMainDB(int numOfAccount, int numOfBatch, int maxCustId);

        int GetCurrentMaxCustId(bool isCustSub);

        List<RawAssociationModel> GetRawAssociation(int numOfAssociation, int numOfBatch);

        List<RawTransactionByIpModel> GetRawTransByIp(int numOfTrans, int numOfBatch);
    }
}
