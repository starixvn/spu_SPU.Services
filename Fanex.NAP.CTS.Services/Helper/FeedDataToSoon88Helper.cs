namespace Fanex.NAP.CTS.Services.Helper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.Security.Cryptography;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.SPU.Core;
    using System.Threading.Tasks;

    public static class FeedDataToSoon88Helper
    {

        public static void FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses)
        {
            var url = "https://player.xoon88.com/fps/updateFPSstatus";
            var updateList = custClasses.Where(c => c.CustomerClass != -1);
            var deleteList = custClasses.Where(c => c.CustomerClass == -1);

            if (updateList.Any())
            {
                UpdateDataToSoon88(updateList, url);
            }

            if (deleteList.Any())
            {
                DeleteDataInSoon88(deleteList, url);
            }

        }

        private static IEnumerable<AssociationWithCategory> GetAssociationWithCategory(IEnumerable<CustomerClassModel> custClasses, bool isDelete)
        {
            var ctsDataCenterService = new DataCenterService();
            var result = ctsDataCenterService.FeedDataToSoon88(custClasses);
            return result;
        }

        private static void UpdateDataToSoon88(IEnumerable<CustomerClassModel> updateList, string url)
        {
            var result = GetAssociationWithCategory(updateList, false);

            var dataGroupByAccount = result.GroupBy(a => a.RegisterName);
            foreach (var groupAcc in dataGroupByAccount)
            {
                var soon88 = StatisticAccountByCategoryInS88(groupAcc.Where(c => c.IsSoon88));
                var others = StatisticAccountByCategoryNotInS88(groupAcc.Where(c => !c.IsSoon88));

                var tempData = new PreparedUpdateDataToSoon88ViewModel()
                {
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = new
                    {
                        soon88,
                        others
                    }
                };

                var finalData = new UpdateDataToSoon88ViewModel()
                {
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = tempData.association,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Consts.SALT)
                };

                var returnedData = ApiHelper.Post<S88Result>(url, finalData);
                Task.WhenAll(returnedData);
            }
        }

        private static void DeleteDataInSoon88(IEnumerable<CustomerClassModel> deleteList, string url)
        {
            var result = GetAssociationWithCategory(deleteList, true);

            foreach (var item in result)
            {
                var tempData = new PreparedDeleteDataToSoon88ViewModel()
                {
                    reason = item.CategoryName,
                    account = item.RegisterName
                };

                var finalData = new DeleteDataToSoon88ViewModel()
                {
                    reason = item.CategoryName,
                    account = item.RegisterName,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Consts.SALT)
                };

                var returnedData = ApiHelper.Post<S88Result>(url, finalData);
                Task.WhenAll(returnedData);

            }
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
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

    }
}
