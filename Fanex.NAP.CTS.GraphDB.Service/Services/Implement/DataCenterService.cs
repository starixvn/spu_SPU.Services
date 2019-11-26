namespace Fanex.NAP.CTS.GraphDB.Service.Implement
{
    using Fanex.NAP.CTS.GraphDB.Service.Interface;
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
