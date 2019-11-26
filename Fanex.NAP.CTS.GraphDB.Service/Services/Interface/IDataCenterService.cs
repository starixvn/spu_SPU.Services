using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.CTS.GraphDB.Service.Interface
{
    public interface IDataCenterService
    {
        DataResult UpdateIsSyncByAssociationId(string associationXml);
    }
}
