using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.CTS.Criteria;
using Fanex.NAP.CTS.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.DataAccess
{
    public class DataCenterDataAccess
    {
        public List<int> TransformDCSAccount(string accountJson)
        {
            var criteria = new TransformDCSAccountToCTSCustomerCriteria
            {
                AccountJson = accountJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria).ToList();
            }
        }

        public void TransformAccountCompleted(string accountJson)
        {
            var repository = new DynamicRepository();

            var criteria = new TransformAccountCompletedCriteria
            {
                AccountJson = accountJson
            };

            repository.Execute(criteria);
        }

        public List<AssociationModel> TransformAssociationByDevice(string associationJson)
        {
            var criteria = new TransformAssociationByDeviceCriteria
            {
               AssociationList = associationJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<AssociationModel>(criteria).ToList();
            }
        }

        public int InsertAssociation(string associationJson)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertAssociationCriteria
            {
                AssociationJson = associationJson
            };

            return repository.Execute(criteria);
        }

        public void TransformAssociationCompleted(string associationJson)
        {
            var repository = new DynamicRepository();

            var criteria = new TransformAssociationCompletedCriteria
            {
                AssociationJson = associationJson
            };

            repository.Execute(criteria);
        }

        public void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIPJson)
        {
            var repository = new DynamicRepository();

            var criteria = new TransformDCSTransactionIPToCTSAssociationByIPCriteria
            {
                TransactionJson = transactionByIPJson
            };

            repository.Execute(criteria);
        }
    }
}
