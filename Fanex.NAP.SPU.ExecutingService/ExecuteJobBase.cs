using Fanex.NAP.SPU.Model;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.ExecutingService
{
    public class ExecuteJobBase : IJob
    {
        protected IUMControlAdapter maintenanceService;

        public ExecuteJobBase()
        {
            maintenanceService = new UMControlAdapter();
        }

        public virtual Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public JobData GetJobDataMapping(IJobExecutionContext context)
        {
            if (context != null)
            {
                var jobDataMap = context.JobDetail.JobDataMap;

                return new JobData
                {
                    Id = jobDataMap.GetInt("Id"),
                    Name = jobDataMap.GetString("Name"),
                    Active = jobDataMap.GetBoolean("Active"),
                    FileName = jobDataMap.GetString("FileName"),
                    Action = jobDataMap.GetString("Action"),
                    IntervalInSecond = jobDataMap.GetInt("IntervalInSecond")
                };
            }

            return null;
        }

        public bool CheckUM()
        {
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }
    }
}
