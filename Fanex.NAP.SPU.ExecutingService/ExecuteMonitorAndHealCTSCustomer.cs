namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteMonitorAndHealCTSCustomer : ExecuteJobBase, IJob
    {
        private readonly IDataCenterService dataCenterService;

        public ExecuteMonitorAndHealCTSCustomer()
        {
            dataCenterService = new DataCenterService();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!this.CheckUM())
                {
                    await ExecuteAPI(context);
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

        private async Task ExecuteAPI(IJobExecutionContext context)
        {
            var jobDataMap = context.JobDetail.JobDataMap;

            try
            {
                if (jobDataMap != null)
                {
                    var rootPath = jobDataMap.GetString(AppSetting.ClusterKey);
                    var isActive = jobDataMap.GetBoolean("Active");
                    var packageSize = jobDataMap.GetInt("PackageSize");

                    if (isActive && !string.IsNullOrWhiteSpace(rootPath))
                    {
                        var apiUrls = rootPath.Split(';');

                        var output = dataCenterService.GetLastUpdatedTime();

                        var updatedCustFromCustInfo = dataCenterService.GetUpdatedCustFromCustInfoHistory(output.LastUpdateTimeCustInfo, output.LastUpdateCustIDCustInfo, packageSize);

                        var updatedCustFromCustomer = dataCenterService.GetUpdatedCustFromCustomerHistory(output.LastUpdateTimeCustomer, output.LastUpdateCustIDCustomer, packageSize);

                        var updatedCustFromProductStatus = dataCenterService.GetUpdatedCustFromProductStatus(output.LastUpdateTimeCustProductStatus, output.LastUpdateCustIDCustProductStatus, packageSize);

                        if (IsValidData(updatedCustFromCustInfo, updatedCustFromCustomer, updatedCustFromProductStatus))
                        {
                            var custInfo = new List<TranformCustModel>()
                            {
                                updatedCustFromCustInfo,
                                updatedCustFromCustomer,
                                updatedCustFromProductStatus
                            };

                            await SendRequest(custInfo, apiUrls, jobDataMap);
                        }
                    }
                }
                else
                {
                    LogService.Error("Update Customer Service - Job Data cannot be null");
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage("Update Customer Service - " + ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        private async Task SendRequest(List<TranformCustModel> custInfo, string[] urlList, JobDataMap mappingData)
        {
            var tasks = new List<Task>();

            var task = Task.Run(() =>
             {
                 var apiUrl = urlList[0];

                 return Executing(custInfo, apiUrl, mappingData.GetString("Action"));
             });

            await Task.Delay(10);

            tasks.Add(task);

            await Task.WhenAll(tasks.ToArray());
        }

        private bool IsValidData(TranformCustModel custFromCustInfo, TranformCustModel custFromCustomer, TranformCustModel custFromProductStatus)
        {
            if ((custFromCustInfo != null && custFromCustInfo.LastCustId != null && custFromCustInfo.LastUpdatedTime != null) 
                || (custFromCustomer != null && custFromCustomer.LastCustId != null && custFromCustomer.LastUpdatedTime != null)
                || (custFromProductStatus != null && custFromProductStatus.LastCustId != null && custFromProductStatus.LastUpdatedTime != null))
            {
                return true;
            }

            return false;
        }
    }
}
