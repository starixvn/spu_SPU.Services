using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class UpdateCustomerInfoCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerInfo")]
        public string CustomerInfo { get; set; }     

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_Account_UpdateCustomerInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
