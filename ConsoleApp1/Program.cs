namespace ConsoleApp1
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var _dataTransformService = new DataTransformService();

            DbSettingProviderManager
                 .StartNewSession()
                 .WithSqlServer()
                 .WithMySql()
                 .UseDefaultDbSettingProvider(resourcePath: AppSettings.AppDataSetting)
                 .UseDefaultConnectionStringProvider()
                 .Run();

            try
            {
                var data = new List<RawTransactionModel>();

                do
                {
                    data = _dataTransformService.GetECRawTrans(2000);

                    if (data != null && data.Any())
                    {
                        _dataTransformService.InsertECRawTrans(JsonConvert.SerializeObject(data));
                        Console.WriteLine(JsonConvert.SerializeObject(data));
                    }
                } while (data != null && data.Any());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
