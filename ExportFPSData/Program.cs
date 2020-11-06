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
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
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
                             Dsn = new Dsn("http://0640ef4e164f4a1da89a5183e1600b78@10.99.99.51:801/123")
                         })
                    });

            List<FPSDataModel> trans = null;

            try
            {
                do
                {                    
                    trans = GetData(Convert.ToInt32(ConfigurationManager.AppSettings["packageSize"])).ToList();                                      

                    if (trans != null && trans.Any())
                    {

                        Console.WriteLine($"Export CSV file is begining({trans.Count} transaction).");

                        var rawTrans = trans.Select(x => TransformRawTransaction(x)).OrderBy(a => a.FPSTransId);

                        var engine = new FileHelperEngine<RawTransactionModel>(Encoding.UTF8);
                        var subcriberName = rawTrans.First().SubscriberName;

                        var folderName = ConfigurationManager.AppSettings["csv-destination-root-path"] + DateTime.Now.ToString("yyyMMdd");
                        var logFileName = ConfigurationManager.AppSettings["logs-path"] + subcriberName + ".log";

                        if (!File.Exists(logFileName))
                        {
                            File.Create(logFileName).Dispose();
                        }

                        if (!Directory.Exists(folderName))
                        {
                            DirectorySecurity securityRules = new DirectorySecurity();
                            securityRules.AddAccessRule(new FileSystemAccessRule(@"Everyone", FileSystemRights.FullControl, AccessControlType.Allow));

                            Directory.CreateDirectory(folderName, securityRules);
                        }

                        var chunkTrans = rawTrans.Chunk(Convert.ToInt32(ConfigurationManager.AppSettings["chunkSize"]));

                        Console.WriteLine($"Total files({subcriberName}):" + chunkTrans.Count());

                        using (StreamWriter w = File.AppendText(logFileName))
                        {
                            w.WriteLine("Total files: " + chunkTrans.Count());

                            w.WriteLine("Total items: " + trans.Count);
                        }

                        var resultLog = new StringBuilder();

                        Parallel.ForEach(chunkTrans, new ParallelOptions { MaxDegreeOfParallelism = 5 }, x =>
                            {
                                try
                                {
                                    var fileName = folderName + "\\" + x.FirstOrDefault().SubscriberName + "-" + x.Min(a => a.FPSTransId) + "-" + x.Max(a => a.FPSTransId) + "-" + x.Count() + ".csv";

                                    engine.WriteFile(fileName, x);

                                    resultLog.Append(fileName);
                                    resultLog.Append("\r\n");
                                }
                                catch (Exception ex)
                                {
                                    var fromTransId = x.Max(a => a.FPSTransId);
                                    var toTransId = x.Min(a => a.FPSTransId);

                                    LogService.Error($"There is something wrong in Export CSV process(trans:{fromTransId} - {toTransId}). Error msg: {ex.Message}");
                                }
                            });

                        using (StreamWriter w = File.AppendText(logFileName))
                        {
                            w.WriteLine(resultLog.ToString());
                        }

                        Console.WriteLine($"Exported {chunkTrans.Count()} files ==> DONE.");

                        Thread.Sleep(2);
                    }
                    else
                    {
                        LogService.Info("There is no data.");
                    }

                    Console.WriteLine($"=======================================================");

                    Thread.Sleep(2);
                }
                while (trans != null && trans.Any());
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message + $"(trans:{trans.Min(a => a.SubTransId)} - {trans.Max(a => a.SubTransId)})");
            }

            Console.WriteLine("Export data is DONE. BYE.");

            //Console.ReadKey();
        }

        private static IEnumerable<FPSDataModel> GetData(int pageSize)
        {
            var criteria = new GetDataCriteria
            {
                PageSize = pageSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FPSDataModel>(criteria);
            }
        }

        private static List<FPSDataModel> ReadCSV(string path)
        {
            //var engine = new FileHelperEngine<FPSDataModel>();

            //return engine.ReadFileAsList(path);

            try
            {
                return File.ReadAllLines(path).Skip(1).Select(a =>
                {
                    var transTime = new DateTime();
                    var createdDate = new DateTime();

                    string[] values = a.Split('|');

                    return new FPSDataModel
                    {
                        TransId = Convert.ToInt32(values[0]),
                        SubTransId = Convert.ToInt32(values[1]),
                        LoginName = values[2].ToString(),
                        SubName = values[3].ToString(),
                        //TransTime = !DateTime.TryParse(a[4].ToString(), out transTime) ? DateTime.Now : transTime,
                        //CreatedDate = !DateTime.TryParse(a[5].ToString(), out createdDate) ? DateTime.Now : createdDate,
                        TransTime = DateTime.Parse(values[4].ToString()),
                        CreatedDate = DateTime.Parse(values[5].ToString()),
                        DeviceCode = values[6].ToString(),
                        IP = values[7].ToString(),
                        ActionName = values[8].ToString(),
                        ActionResult = values[9].ToString(),
                        Url = values[10].ToString(),
                        UserAgent = values[11].ToString()
                    };
                }).ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

            return new List<FPSDataModel>();
        }

        private static RawTransactionModel TransformRawTransaction(FPSDataModel source)
        {
            source.DeviceCode = string.Empty;

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
                CreatedDate = Convert.ToDateTime(source.TransTime.ToString("yyyy-MM-dd")),
                IP = source.IP,
                IPId = source.IP.Ip2Long().IpNumber,
                PluginID = 0,
                URL = string.IsNullOrWhiteSpace(url) ? ConvertNA2Empty(SubStringUrl(source.Url)) : SubStringUrl(url),
                Action = source.ActionName,
                ActionResult = source.ActionResult,
                UserAgent = string.IsNullOrWhiteSpace(userAgent) ? ConvertNA2Empty(StripUnicodeCharactersFromString(source.UserAgent)) : StripUnicodeCharactersFromString(userAgent),
                FingerprintCode = deviceInfo.DeviceFPCode != "N/A" ? deviceInfo.DeviceFPCode : null,
                FingerprintMoreInfo = null,
                TransId = source.TransId,
                FPSTransId = source.SubTransId
            };
        }

        private static string SubStringUrl(string url)
        {
            return url.IndexOf('?') != -1 ? url.Substring(0, url.IndexOf('?')) : url;
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

        private static String StripUnicodeCharactersFromString(string inputValue)
        {
            return Regex.Replace(inputValue, @"[^\u0000-\u007F]", String.Empty);
        }
    }
}
