namespace Fanex.NAP.SPU.MonitoringService.Services
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.NAP.SPU.MonitoringService.Criteria;
    using Fanex.NAP.SPU.MonitoringService.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataAccessService
    {
        public DataAccessService()
        {
            DbSettingProviderManager
                 .StartNewSession()
                 .WithSqlServer()
                 .UseDefaultDbSettingProvider("~/App_Data/SPNames.xml")
                 .UseDefaultConnectionStringProvider()
                 .Run();
        }

        public IEnumerable<QuartzTrigger> GetAllSimpleTriggers()
        {
            var criteria = new GetSimpleTriggerCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<QuartzTrigger>(criteria);
            }
        }
    }
}
