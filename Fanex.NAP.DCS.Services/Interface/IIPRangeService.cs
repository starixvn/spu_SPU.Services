using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Services.Interface
{
   public interface IIPRangeService
    {
        int UpdateIpRange(DateTime updatedDate);

        void SaveCSV(IEnumerable<IpRangeModel> IpRanges, DateTime updatedDate, string filePath);

        void SaveRangeIpId(int fromId, int toId, string filePath);

        void GetRangeIpId(ref int fromId, ref int toId, int size, string filePath);

        List<IpRangeModel> GetIpV6Range(int fromId, int toId);

        DateTime GetIPDBVersion();

        DateTime GetLatestUpdatedDateIPTableLog();

        DataResult InsertNewTableLog(DateTime updatedDate);
    }
}
