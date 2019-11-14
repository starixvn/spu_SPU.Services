using Fanex.NAP.CTS.DataAccess;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Services
{
    public class DataCenterService : IDataCenterService
    {
        private readonly DataCenterDataAccess dataCenterDataAccess;

        public DataCenterService()
        {
            dataCenterDataAccess = new DataCenterDataAccess();
        }

        public void InsertAccount(List<RawAccountModel> accountList)
        {
            var accountJson = JsonConvert.SerializeObject(accountList);

            var result = dataCenterDataAccess.InsertAccount(accountJson);

            if (result != 0)
            {
                dataCenterDataAccess.TransformAccountCompleted(accountJson);
            }
        }
    }
}
