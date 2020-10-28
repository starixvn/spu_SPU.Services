
namespace Fanex.NAP.CTS.Services.Helper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.Security.Cryptography;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.SPU.Core;

    public static class FeedDataToSoon88Helper
    {
  
        public static void FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses)
        {
            var updateList = custClasses.Where(c => c.CustomerClass != -1);
            var deleteList = custClasses.Where(c => c.CustomerClass == -1);

            var data = GetAssociationWithCategory(updateList);

        }

        private static IEnumerable<AssociationWithCategory> GetAssociationWithCategory(IEnumerable<CustomerClassModel> custClasses)
        {
            var ctsDataCenterService = new DataCenterService();

            var result = ctsDataCenterService.FeedDataToSoon88(custClasses);

            var dataGroupByAccount = result.GroupBy(a => a.RegisterName);

            foreach (var groupAcc in dataGroupByAccount)
            {
                var soon88 = StatisticAccountByCategoryInS88(groupAcc.Where(c => c.IsSoon88));
                var others = StatisticAccountByCategoryNotInS88(groupAcc.Where(c => !c.IsSoon88));

                var tempData = new PreparedDataToSoon88ViewModel()
                {
                    Action = "block",
                    Reason = groupAcc.FirstOrDefault().CategoryName,
                    Account = groupAcc.Key,
                    Association = new
                    {
                        soon88,
                        others
                    }
                };

                var a = new FeedDataToSoon88ViewModel()
                {
                    action = "block",
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = tempData.Association,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Constants.SALT)
                };
            }

            return result;
        }

        private static Dictionary<string, int> StatisticAccountByCategoryNotInS88(IEnumerable<AssociationWithCategory> assList)
        {
            var statResult = new Dictionary<string, int>();
            foreach (var item in assList.GroupBy(a => a.AssCategoryName))
            {
                statResult.Add(item.Key, item.Count());
            }

            return statResult;
        }

        private static Dictionary<string, IEnumerable<string>> StatisticAccountByCategoryInS88(IEnumerable<AssociationWithCategory> assList)
        {
            var statResult = new Dictionary<string, IEnumerable<string>>();
            foreach (var item in assList.GroupBy(a => a.AssCategoryName))
            {
                statResult.Add(item.Key, item.Select(b => b.AssRegisterName));
            }

            return statResult;
        }

        private static string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
