using Fanex.NAP.FPS.Renovation.Model;
using System.Collections.Generic;

namespace Fanex.NAP.FPS.Renovation.Services.Interface
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
    }
}
