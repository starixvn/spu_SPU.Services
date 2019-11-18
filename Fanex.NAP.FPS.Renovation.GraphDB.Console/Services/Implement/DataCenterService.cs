namespace Fanex.NAP.CTS.Renovation.GraphDB.Console.Services.Implement
{
    using Fanex.NAP.CTS.Renovation.GraphDB.Console.Services.Interface;
    using Fanex.NAP.DCS.DataAccess;
    using Fanex.NAP.SPU.Model;

    public class DataCenterService : IDataCenterService
    {
        private readonly DataCenterDataAccess dataCenter;

        public DataCenterService()
        {
            dataCenter = new DataCenterDataAccess();
        }

        public DataResult UpdateIsSyncByAssociationId(string associationXml)
        {
            return dataCenter.UpdateIsSyncGraphDBByAssociationId(associationXml);
        }
    }
}
