using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetTotalAssociationByCustomerCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public int RoleId { get; set; }

        public override string GetSettingKey()
        {
            return "DC_GetTotalAssociationByCustomers";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
