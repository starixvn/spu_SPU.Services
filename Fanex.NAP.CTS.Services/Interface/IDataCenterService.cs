using Fanex.NAP.CTS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Services.Interface
{
    public interface IDataCenterService
    {
        List<int> TransformDCSAccount(List<RawAccountModel> accountList);

        void InsertAssociation(List<AssociationModel> associationList);

        List<Model.AssociationModel> GetAssociationByAccounts(List<RawAssociationModel> rawAssociation);

        void TransformAccountCompleted(List<RawAccountModel> insertedAccount);
    }
}
