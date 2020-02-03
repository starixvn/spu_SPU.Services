using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.DeviceDetector.Factory;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.DCS.Core;
using Fanex.NAP.DCS.Model;
using FileHelpers;
using Sentry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExportFPSData
{
   static class Program
    {
        static void Main(string[] args)
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer()
                .UseDefaultDbSettingProvider(resourcePath: "~/App_Data/SPNames.xml")
                .UseDefaultConnectionStringProvider()
                .Run();

            LogManager.SetDefaultLogCategory("Export-FPS-Data")
                    .Use(new List<ILogEngine>
                    {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn("http://5db4409de047476aacba9ee1f3c70e26@10.99.99.37:801/63")
                         })
                    });

            List<FPSDataModel> trans = null;

            try
            {
                do
                {
                    trans = GetData(Convert.ToInt32(ConfigurationManager.AppSettings["packageSize"]));

                    if (trans != null && trans.Any())
                    {
                        Console.WriteLine($"Export CSV file is begining({trans.Count} transaction).");

                        var rawTrans = trans.Select(x => TransformRawTransaction(x));
                        var counter = 0;

                        if (rawTrans != null && rawTrans.Any())
                        {
                            var engine = new FileHelperEngine<RawTransactionModel>();
                            var chunkTrans = rawTrans.Chunk(Convert.ToInt32(ConfigurationManager.AppSettings["chunkSize"]));
                            var random = new Random();

                            Parallel.ForEach(chunkTrans, new ParallelOptions { MaxDegreeOfParallelism = 5 }, x =>
                            {
                                try
                                {
                                    var folderName = ConfigurationManager.AppSettings["csv-destination-root-path"] + DateTime.Now.ToString("yyyMMdd");

                                    if (!Directory.Exists(folderName))
                                    {
                                        Directory.CreateDirectory(folderName);
                                    }

                                    var fileName = string.Empty;

                                    do
                                    {
                                        fileName = folderName + "//" + DateTime.Now.ToString("yyyyMMddhhmmss") + random.Next(10000) + ".csv";
                                    }
                                    while (File.Exists(fileName));

                                    engine.WriteFile(fileName, x);

                                    counter++;
                                }
                                catch (Exception ex)
                                {
                                    var fromTransId = x.Max(a => a.TransId);
                                    var toTransId = x.Min(a => a.TransId);

                                    LogService.Error($"There is something wrong in Export CSV process(trans:{fromTransId} - {toTransId}). Error msg: {ex.Message}");
                                }
                            });
                        }

                        Console.WriteLine($"Exported {counter} files ==> DONE.");

                        Thread.Sleep(2);
                    }
                    else
                    {
                        LogService.Info("There is no data.");
                    }

                    Console.WriteLine($"=======================================================");
                }
                while (trans != null && trans.Any());
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            Console.WriteLine("Export data is DONE. BYE.");
        }

        private static List<FPSDataModel> GetData(int pageSize)
        {
            var criteria = new GetDataCriteria
            {
                PageSize = pageSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FPSDataModel>(criteria).ToList();
            }
        }

        private static RawTransactionModel TransformRawTransaction(FPSDataModel source)
        {
            var invalidDevice = string.Empty;
            var deviceInfo = GetDeviceInfo(source.DeviceCode);
            var url = Fanex.NAP.DCS.Core.Utilities.GetURLFromDeviceCode(source.DeviceCode);
            var userAgent = Fanex.NAP.DCS.Core.Utilities.GetUserAgentFromDeviceCode(source.DeviceCode);
            var transBit = new BitArray(16);

            if (string.IsNullOrWhiteSpace(deviceInfo.DeviceNo) || deviceInfo.DeviceNo == "N/A")
            {
                deviceInfo.DeviceNo = null;
                transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
            }
            else
            {
                if (!Fanex.NAP.DCS.Core.Utilities.IsGuidDeviceId(deviceInfo.DeviceNo) || deviceInfo.InvalidDeviceId)
                {
                    invalidDevice = source.DeviceCode;
                    deviceInfo.DeviceNo = null;
                    transBit.Set((int)TransStatus.InvalidDI, true);
                }
            }

            if (ContainsUnicodeCharacter(source.LoginName))
            {
                transBit.Set((int)TransStatus.InvalidLoginName, true);
            }

            return new RawTransactionModel
            {
                DeviceCode = deviceInfo.DeviceNo,
                InvalidDevice = string.IsNullOrWhiteSpace(invalidDevice) ? null : invalidDevice,
                TransStatus = Fanex.NAP.DCS.Core.Utilities.ConvertBitArray2Int(transBit),
                LoginName = source.LoginName.Trim(),
                SubscriberName = source.SubName,
                TransTime = Convert.ToDateTime(source.TransTime.ToString("yyyy-MM-dd HH:mm:ss.ffff")),
                CreatedDate = Convert.ToDateTime(source.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")),
                IP = source.IP,
                IPId = source.IP.Ip2Long().IpNumber,
                PluginID = 0,
                URL = string.IsNullOrWhiteSpace(url) ? ConvertNA2Empty(deviceInfo.HRef) : url.Split('?')[0],
                Action = source.ActionName,
                ActionResult = source.ActionResult,
                UserAgent = string.IsNullOrWhiteSpace(userAgent) ? ConvertNA2Empty(deviceInfo.UserAgent) : userAgent,
                FingerprintCode = deviceInfo.DeviceFPCode != "N/A" ? deviceInfo.DeviceFPCode : null,
                FingerprintMoreInfo = null,
                TransId = source.TransId
            };
        }

        private static DeviceInfoModel GetDeviceInfo(string deviceCode)
        {
            if (string.IsNullOrWhiteSpace(deviceCode))
            {
                return TransformEmptyDeviceInfo();
            }

            var deviceInfo = DeviceFactory.GetDeviceInfo(deviceCode);

            if (deviceInfo != null)
            {
                return TransformDeviceInfo(deviceInfo);
            }

            return TransformEmptyDeviceInfo();
        }

        private static DeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new DeviceInfoModel
            {
                AppVersion = string.Empty,
                CapturedDate = 0,
                DeviceFPCode = string.Empty,
                DeviceNo = string.Empty,
                HRef = string.Empty,
                NA = string.Empty,
                Screen = new ScreenInfoModel
                {
                    ColorDepth = 0,
                    Height = 0,
                    Width = 0
                },
                TimeZone = string.Empty,
                UserAgent = string.Empty,
                InvalidDeviceId = true
            };
        }

        private static DeviceInfoModel TransformDeviceInfo(Fanex.DeviceDetector.Models.DeviceInfo deviceInfo)
        {
            return new DeviceInfoModel
            {
                AppVersion = deviceInfo.AppVersion,
                CapturedDate = deviceInfo.CapturedDate,
                DeviceFPCode = deviceInfo.DeviceCode,
                DeviceNo = deviceInfo.DeviceId,
                HRef = deviceInfo.HRef,
                NA = deviceInfo.NA,
                TimeZone = deviceInfo.TimeZone,
                UserAgent = deviceInfo.UserAgent,
                Screen = deviceInfo.Screen == null
                    ? new ScreenInfoModel
                    {
                        ColorDepth = 0,
                        Height = 0,
                        Width = 0
                    }
                    : new ScreenInfoModel
                    {
                        ColorDepth = deviceInfo.Screen.ColorDepth,
                        Height = deviceInfo.Screen.Height,
                        Width = deviceInfo.Screen.Width
                    }
            };
        }

        private static string ConvertNA2Empty(string data)
        {
            return data != "N/A" ? data : null;
        }

        private static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }
    }
}
