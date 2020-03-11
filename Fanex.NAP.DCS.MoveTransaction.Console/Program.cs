using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.SPU.Core;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.MoveTransaction.Console
{
   static class Program
    {
        static void Main(string[] args)
        {
            DbSettingProviderManager
               .StartNewSession()
               .WithMySql()              
               .UseDefaultDbSettingProvider(resourcePath: AppSettings.AppDataSetting)
               .UseDefaultConnectionStringProvider()
               .Run();

            LogManager
               .SetDefaultLogCategory(AppSettings.FanexLogCategory)
               .Use(new List<ILogEngine>
               {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(AppSettings.SentryLogging)
                    })
               });




        }
    }
}
