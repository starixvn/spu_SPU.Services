namespace Fanex.NAP.CTS.Services.Implement
{
    using Fanex.NAP.CTS.DataAccess;

    public interface IAssociationService
    {
        void InsertAssociationDevice(int batchSize);
    }

    public class AssociationService : IAssociationService
    {
        private readonly DataCenterDataAccess dataCenterDataAccess;

        public AssociationService()
        {
            dataCenterDataAccess = new DataCenterDataAccess();
        }

        public void InsertAssociationDevice(int batchSize)
        {
            dataCenterDataAccess.InserAssociationDevice(batchSize);
        }
    }
}
