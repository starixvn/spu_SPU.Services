using Fanex.NAP.CTS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Services.Interface
{
    public interface IDataCenterService
    {
        void TransformDCSAccount(List<RawAccountModel> accountList);

        List<int> TransformCustomerFromMainDB(List<CustomerMainDbModel> customerList);

        void InsertAssociation(List<AssociationModel> associationList);

        List<AssociationModel> TransformAssociationByDevice(List<RawAssociationModel> rawAssociation);

        void TransformAssociationCompleted(List<AssociationModel> associationList);

        void TransformDCSTransactionIPToCTSAssociationByIP(List<RawTransactionByIpModel> transactionByIpList);
    }
}
