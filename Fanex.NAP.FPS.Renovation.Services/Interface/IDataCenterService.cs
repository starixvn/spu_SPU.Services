using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.DCS.Services.Interface
{
    public interface IDataCenterService
    {
        List<NewCustomerModel> GetNewCustomer(int size);

        DataResult UpdateCustomerInfo(List<CustomerInfoModel> customerInfo);

        List<CustomerInfoModel> GetCustomerInfo(List<NewCustomerModel> newCustomers);

        List<AssociationModel> GetAssociationForGrapDB(int isSync, int size);

        DataResult UpdateIsSyncByAssociationId(string association);


        List<int> GetUpdatedAccount(int size);

        DataResult UpdateAccountId(string accountIdJson);

        List<DevicePackage> GetStagingTransIdForDevice(int size, int noOfBatch);

        DataResult InsertDevice(List<DevicePackage> transId);

        List<RawAccountModel> GetRawAccount(int numOfAccount, int numOfBatch);

        List<RawAssociationModel> GetRawAssociation(int numOfAssociation, int numOfBatch);
    }
}
