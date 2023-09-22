using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Extensions;
using Fanex.NAP.DCS.Model;
using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;
using Fanex.Logging;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.DCS.Core;
using System.Threading;

namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Implement
{
    public class ExtraSubsTransactionFactory : AbstractFactoryTransaction
    {
        private readonly ICapturingDataService _capturingDataService;

        public ExtraSubsTransactionFactory()
        {
            _capturingDataService  = new CapturingDataService();
        }
        protected override void InsertTransaction(List<RawTransactionModel> transactions)
        {
            try
            {
                var ipInfoList = GetIpInfos(transactions);
                if (ipInfoList != null && ipInfoList.Any())
                {
                    var newTransOfExtraSubs = GetNewTransExtraSub(transactions, ipInfoList);
                    ProcessInsertTransaction(transactions);
                }
                else
                {
                    ProcessInsertTransaction(transactions);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        protected override void DelayTask(List<RawTransactionModel> transactions, List<Task> tasks, CancellationTokenSource tokenSource)
        {

            if (!tasks.Any())
            {
                LogService.Info("start DelayTaskAddTransactionExtraSubs at: " + DateTime.Now);
                var delayTime = AppSettings.Interval * 60 * 1000;
                var taskDelay = Task.Delay((int)delayTime).ContinueWith((task) =>
                {
                    try
                    {
                        ProcessDelayedTask(transactions, tasks);
                    }
                    catch (OperationCanceledException)
                    {
                        LogService.Info("Task of add transaction extra sub list was canceled.");
                    }
                }, tokenSource.Token);
                tasks.Add(taskDelay);
            }
        }

        protected override int GetBatchThreshold() => AppSettings.TransOfExtraSubsBatch;

        protected override bool IsTransOfDBExtra() => true;

        private void ProcessInsertTransaction(List<RawTransactionModel> transactions)
        {
            try
            {
                _capturingDataService.InsertTransaction(transactions, IsTransOfDBExtra());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        private void ProcessDelayedTask(List<RawTransactionModel> transactions, List<Task> tasks)
        {
            if (transactions.Any())
            {
                var ipInfoList = GetIpInfos(transactions);
                if (ipInfoList != null && ipInfoList.Any())
                {
                    var newTransOfExtraSubs = GetNewTransExtraSub(transactions, ipInfoList);
                    _capturingDataService.InsertTransaction(newTransOfExtraSubs, IsTransOfDBExtra());
                }
                else
                {
                    _capturingDataService.InsertTransaction(transactions, IsTransOfDBExtra());
                }
            }
            transactions.Clear();
            tasks.Clear();
            LogService.Info("end TriggerTaskAddTransactionExtraSubs at: " + DateTime.Now);
        }

        private IEnumerable<IpInfoModel> GetIpInfos(List<RawTransactionModel> transactions)
        {
            var ipAddressList = transactions.Where(x => x.IPId != -1 && !x.IsIPV6).Select(x => x.IPId).ToList();
            var ipAddressJoined = string.Join(",", ipAddressList);
            var ipInfoList = _capturingDataService.GetIpInfoList(ipAddressJoined);
            return ipInfoList;
        }

        private List<RawTransactionModel> GetNewTransExtraSub(List<RawTransactionModel> transactions, IEnumerable<IpInfoModel> ipInfoList)
        {
            var newTransOfExtraSubs = transactions.Select(x =>
            {
                var ipInfo = ipInfoList.FirstOrDefault(y => x.IPId == y.IP);

                x.Country = ipInfo?.Country;
                x.CountryCode = ipInfo?.CountryCode;
                x.Region = ipInfo?.Region;
                x.City = ipInfo?.City;
                x.ISP = ipInfo?.ISP;
                x.IPInfoCode = Cipher.Hashing2Int(string.Join(";", ipInfo?.CountryCode, ipInfo?.Region, ipInfo?.City, ipInfo?.ISP));

                return x;
            }).ToList();
            return newTransOfExtraSubs;
        }
    }
}
