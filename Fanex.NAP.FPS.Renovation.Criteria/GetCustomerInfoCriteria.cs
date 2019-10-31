using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetCustomerInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "customerList")]
        public string CustomerList { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_GetCustomerInfo40";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
