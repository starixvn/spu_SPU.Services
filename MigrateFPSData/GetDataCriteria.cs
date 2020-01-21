using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateFPSData
{
    public class GetDataCriteria : CriteriaBase
    {
        public int PageSize { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_MS_GetDataForMigrate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
