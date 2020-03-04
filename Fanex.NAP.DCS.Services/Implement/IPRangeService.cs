namespace Fanex.NAP.DCS.Services.Implement
{
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.DCS.DataAccess;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.Services.Interface;
    using Fanex.NAP.SPU.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class IPRangeService : IIPRangeService
    {
        private readonly DataAccess.IPRangeDataAccess ipRangeDataAccess;

        public IPRangeService()
        {
            ipRangeDataAccess = new IPRangeDataAccess();
        }

        public int UpdateIpRange(DateTime updatedDate)
        {
            var result = ipRangeDataAccess.UpdateIPRange(updatedDate);

            return result;
        }

        public DateTime GetIPDBVersion() => ipRangeDataAccess.GetIPDBVersion();

        public DateTime GetLatestUpdatedDateIPTableLog() => ipRangeDataAccess.GetLatestUpdatedDataIPTableLog();

        public DataResult InsertNewTableLog(DateTime updatedDate)
        {
            return ipRangeDataAccess.InsertNewTableLog(updatedDate);
        }

        private static Int64 EncodeIPAttribute(IpRangeModel ipRangeModel)
        {
            return Cipher.Hashing2Int(string.Format("{0}{1}{2}{3}{4}{5}{6}",
                ipRangeModel.FromIP,
                ipRangeModel.ToIP,
                ipRangeModel.CountryCode,
                ipRangeModel.CountryName,
                ipRangeModel.Region,
                ipRangeModel.City,
                ipRangeModel.ISPName));
        }

        public List<IpRangeModel> GetIpV6Range(int fromId, int toId)
        {
            return ipRangeDataAccess.GetIpV6Range(fromId, toId);
        }

        public void SaveCSV(IEnumerable<IpRangeModel> IpRanges, DateTime updatedDate, string filePath)
        {
            var ipRangeEncode = IpRanges.Select(x =>
            {
                x.IPRangeLocationCode = EncodeIPAttribute(x);
                x.FromIP = x.FromIP.Trim();
                x.ToIP = x.ToIP.Trim();
                return x;
            }).ToList();

            foreach (var s in ipRangeEncode)
            {
                var ipRangeResult = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                   s.IPRangeLocationCode, s.FromIP, s.ToIP, s.CountryCode, s.CountryName, s.Region, s.City, s.ISPName, 0, updatedDate.ToString("yyyy-MM-dd HH:mm:ss"));

                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(ipRangeResult);
                }
            }
        }

        public void SaveRangeIpId(int fromId, int toId, string filePath)
        {
            var content = new StringBuilder();
            content.Append(fromId);
            content.Append(System.Environment.NewLine);
            content.Append(toId);

            File.WriteAllText(filePath, content.ToString());
        }

        public void GetRangeIpId(ref int fromId, ref int toId, int size, string filePath)
        {
            if(File.Exists(filePath))
            {
                var result = File.ReadAllLines(filePath);

                if (result.Any())
                {
                    fromId = Convert.ToInt32(result[0]);
                    toId = Convert.ToInt32(result[1]);
                }
                else
                {
                    fromId = 1;
                    toId = size;
                }
            }           
            else
            {
                fromId = 1;
                toId = size;
            }
        }
    }
}
