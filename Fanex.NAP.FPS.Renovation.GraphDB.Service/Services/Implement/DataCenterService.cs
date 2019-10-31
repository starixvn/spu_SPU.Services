namespace Fanex.NAP.FPS.Renovation.GraphDB.Service.Services.Implement
{
    using Fanex.NAP.FPS.Renovation.DataAccess;
    using Fanex.NAP.FPS.Renovation.GraphDB.Service.Services.Interface;
    using Fanex.NAP.FPS.Renovation.Model;

    public class DataCenterService : IDataCenterService
    {
        private readonly DataAccess.DataCenterDataAccess dataCenter;

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
