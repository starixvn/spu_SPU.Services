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

        public List<int> FillUpCustomerFromMainDB(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.FillUpCustomerFromMainDB(customerJson);
        }

        public List<CTSCustomer> GetCustomerForUpdateUsername2(int rowSize, bool isUpdateCustSub, int maxCTSCustId)
        {
            return dataCenterDataAccess.GetCustomersForUpdateUsername2(rowSize, isUpdateCustSub, maxCTSCustId);
        }

        public List<CustomerMainDbModel> GetUsername2FromMainDb(string custIds, bool isCustSub)
        {
            return dataCenterDataAccess.GetUsername2FromMainDb(custIds,isCustSub);
        }

        public int UpdateUsername2(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.UpdateUsername2(customerJson);
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
            dataCenterDataAccess.TransformDCSTransactionIPToCTSAssociationByIP(transactionByIpJson);
        }

        public void TransformAssociationAffectedEvidence(int batchSize)
        {
            dataCenterDataAccess.TransformAssociationAffectedEvidence(batchSize);
        }
    }
}
