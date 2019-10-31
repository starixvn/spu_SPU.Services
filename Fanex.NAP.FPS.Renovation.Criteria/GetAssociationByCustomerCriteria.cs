using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetAssociationByCustomerCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public int RoleId { get; set; }

        public override string GetSettingKey()
        {
            return "DC_GetAssociationByCustomers";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
