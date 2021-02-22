using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.CMS.VirtualSetting.Model;
using Fanex.NAP.CMS.VirtualSetting.Service;
using Fanex.NAP.SPU.Core;
using System;
using System.Linq;

namespace Fanex.NAP.CMS.VRMemberBigStake.Console
{
    public class Service : BaseService
    {

        private readonly VRMemberBigStakeService memberBigStakeService;

        private VRGetMemberBigStakePerformanceModel getMemberBigStakePerformanceModel;

        public Service()
        {
            memberBigStakeService = new VRMemberBigStakeService();
        }

        public Service StartTime()
        {
            LogService.Info($"VR - Update Member Big Stake - StartTime: {DateTime.Now}.");

            return this;
        }

        public Service InsertVRMemberBigStake()
        {
            memberBigStakeService.InsertVRMemberBigStake();

            return this;

        }

        public Service GetMemberBigStakePerformance()
        {
            getMemberBigStakePerformanceModel = memberBigStakeService.GetMemberBigStakePerformance();

            return this;
        }

        public Service UpdateMemberBigStakeFirstBetDate()
        {
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    if (getMemberBigStakePerformanceModel != null && getMemberBigStakePerformanceModel.FirstBetDateList != null && getMemberBigStakePerformanceModel.FirstBetDateList.Any())
                    {
                        getMemberBigStakePerformanceModel.FirstBetDateList.GroupBy(x => x.FirstBetDate).ToList().ForEach(r =>
                        {
                            var custIds = r.Select(x => x.CustId);

                            custIds.ChunkSize(500).ToList().ForEach(x =>
                            {
                                memberBigStakeService.UpdateVRMemeberBigStakeFirstBetDate(x, 1, r.Key);
                            });
                        });
                    }

                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Update First Bet Date Member Big Stake - Retry: {countRetry} message:  {logMessage}");
                    
                    countRetry += 1;

                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw;
                    }
                }
            }

                                          

            return this;
        }

        public Service UpdateMemberBigStakeStatus()
        {

            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    if (getMemberBigStakePerformanceModel != null && getMemberBigStakePerformanceModel.StatusList != null && getMemberBigStakePerformanceModel.StatusList.Any())
                    {
                        var custIdByStatus = getMemberBigStakePerformanceModel.StatusList.Select(x => x.CustId);

                        custIdByStatus.ChunkSize(500).ToList().ForEach(x => {
                            memberBigStakeService.UpdateVRWhitelistMemberBigStake(custIdByStatus, 1);
                        });
                    }

                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Update White list Member Big Stake - Retry: {countRetry} message:  {logMessage}");

                    countRetry += 1;

                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw;
                    }
                }
            }
            
               
            return this;
        }

    }
}
