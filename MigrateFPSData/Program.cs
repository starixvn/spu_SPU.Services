using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.DeviceDetector.Factory;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.DCS.Core;
using Fanex.NAP.DCS.Model;
using Fanex.NAP.SPU.Core;
using FileHelpers;
using Renci.SshNet;
using Sentry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MigrateFPSData
{
    static class Program
    {
        private static string sshScript = "sh /etc/init.d/ImportMigrateFPSData.sh {0} {1}";

        public static void Main(string[] args)
        {
            var url = "https://www.translatoruser-int.com/translate?&to=en&csId=64d24533-890f-49dd-8691-ee1afeb91025&usId=a86c48b6-3cfa-4eee-8138-30f6998a3ad4&ref=SERP&refd=www.bing.com&dl=en&ac=true&dt=2019%2F9%2F30%204%3A5&h=cX4XTdV6zWpfPfcjVD39fetzJefX0RBt&a=https%3A%2F%2Fwww.ultra88.com%2F";
            //var url = "https://www.translatoruser-int.com/translate";
            var result = url.IndexOf('?') != -1 ? url.Substring(0, url.IndexOf('?')) : url ;

            Console.WriteLine(result);

            Console.ReadKey();
        }

        private static bool ImportCSV(ConnectionInfo connectionInfo, string csvFolder, string csvFileName)
        {
            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();

                var scriptCommand = string.Format(sshScript, csvFolder, csvFileName);

                var sshCommand = client.RunCommand(scriptCommand);

                if (!string.IsNullOrWhiteSpace(sshCommand.Error))
                {
                    LogService.Error(sshCommand.Error);

                    return false;
                }

                return true;
            }
        }

        private static List<SourceModel> GetData(int pageSize)
        {
            var criteria = new GetDataCriteria
            {
                PageSize = pageSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SourceModel>(criteria).ToList();
            }
        }

        private static RawTransactionModel TransformRawTransaction(SourceModel source)
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
