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

        public void TransformDCSAccount(List<RawAccountModel> accountList)
        {
            var accountJson = JsonConvert.SerializeObject(accountList);

            dataCenterDataAccess.TransformDCSAccount(accountJson);
        }

        public List<int> TransformCustomerFromMainDB(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.TransformCustomerFromMainDB(customerJson);
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

        public void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIpJson)
        {
            //var transactionByIpJson = JsonConvert.SerializeObject(transactionByIpList);

            dataCenterDataAccess.TransformDCSTransactionIPToCTSAssociationByIP(transactionByIpJson);
        }
    }
}
