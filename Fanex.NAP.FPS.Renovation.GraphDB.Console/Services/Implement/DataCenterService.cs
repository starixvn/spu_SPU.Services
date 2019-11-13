namespace Fanex.NAP.FPS.Renovation.GraphDB.Console.Services.Implement
{
    using Fanex.NAP.FPS.Renovation.DataAccess;
    using Fanex.NAP.FPS.Renovation.GraphDB.Console.Services.Interface;
    using Fanex.NAP.FPS.Renovation.Model;

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
