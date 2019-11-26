using Fanex.NAP.CTS.DataAccess;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services.Interface;
using Fanex.NAP.SPU.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.Services
{
    public class DataCenterService : IDataCenterService
    {
        private readonly DataCenterDataAccess dataCenterDataAccess;

        public DataCenterService()
        {
            dataCenterDataAccess = new DataCenterDataAccess();
        }

        public List<AssociationModel> TransformAssociationByDevice(List<RawAssociationModel> rawAssociation)
        {
            var associationJson = JsonConvert.SerializeObject(rawAssociation);

            return dataCenterDataAccess.TransformAssociationByDevice(associationJson);
        }

        public List<int> TransformDCSAccount(List<RawAccountModel> accountList)
        {
            var accountJson = JsonConvert.SerializeObject(accountList);

            return dataCenterDataAccess.TransformDCSAccount(accountJson);            
        }

        public void InsertAssociation(List<AssociationModel> associationList)
        {
            var associationJson = JsonConvert.SerializeObject(associationList);

            dataCenterDataAccess.InsertAssociation(associationJson);           
        }       

        public void TransformAssociationCompleted(List<AssociationModel> associationList)
        {
            var associationJson = JsonConvert.SerializeObject(associationList);

            dataCenterDataAccess.TransformAssociationCompleted(associationJson);
        }

        public void TransformAccountCompleted(List<RawAccountModel> insertedAccount)
        {
            var insertedAccountJson = JsonConvert.SerializeObject(insertedAccount);

            dataCenterDataAccess.TransformAccountCompleted(insertedAccountJson);
        }

        public void TransformDCSTransactionIPToCTSAssociationByIP(List<RawTransactionByIpModel> transactionByIpList)
        {
            var transactionByIpJson = JsonConvert.SerializeObject(transactionByIpList);

            dataCenterDataAccess.TransformDCSTransactionIPToCTSAssociationByIP(transactionByIpJson);
        }
    }
}
