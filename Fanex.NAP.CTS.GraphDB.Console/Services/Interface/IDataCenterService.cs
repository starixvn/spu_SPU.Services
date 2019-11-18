using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.CTS.Renovation.GraphDB.Console.Services.Interface
{
    public interface IDataCenterService
    {
        DataResult UpdateIsSyncByAssociationId(string associationXml);
    }
}
