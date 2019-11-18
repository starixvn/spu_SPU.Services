using Fanex.NAP.CTS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Services.Interface
{
    public interface IDataCenterService
    {
        void InsertAccount(List<RawAccountModel> accountList);

        void InsertAssociation(List<Model.AssociationModel> associationList);

        List<Model.AssociationModel> GetAssociationByAccounts(List<RawAssociationModel> rawAssociation);

        void TransformAssociationCompleleted(List<RawAssociationModel> rawAssociation);
    }
}
