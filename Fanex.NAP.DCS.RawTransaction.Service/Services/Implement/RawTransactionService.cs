namespace Fanex.NAP.DCS.RawTransaction.Service.Services.Implement
{
    using Fanex.NAP.DCS.Core;
    using System.Collections;
    using System;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.RawTransaction.Service.Services.Interface;
    using Fanex.Oneyed.Infrastructure.Models;
    using Newtonsoft.Json;
    using System.Linq;
    using Fanex.NAP.SPU.Core;
    using Utilities = Core.Utilities;
    using Fanex.Logging;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Configuration;

    public class RawTransactionService : IRawTransactionService
    {
        private readonly ICapturingDataService _capturingDataService;

        public RawTransactionService()
        {
            _capturingDataService = new CapturingDataService();
        }
        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            LogMessage(message);

            var invalidDevice = string.Empty;
            var deviceCode = Utilities.GetDeviceCodeFromCapturedData(message.Device);
            var deviceInfo = _capturingDataService.GetDeviceInfo(deviceCode);
            var url = Utilities.GetURLFromDeviceCode(message.Device);
            var userAgent = Utilities.GetUserAgentFromDeviceCode(message.Device);
            var transBit = new BitArray(16);

            var flaggedCode = deviceInfo.BotD?.Result?.Bot == true ? deviceInfo.BotD.Result.BotKind : string.Empty;
            if (string.IsNullOrEmpty(flaggedCode))
            {
                var browserless = Browserless(userAgent, deviceInfo.DeviceNo);
                flaggedCode = browserless.ToString();
            }

            if (string.IsNullOrWhiteSpace(deviceInfo.DeviceNo) || deviceInfo.DeviceNo == "N/A")
            {
                deviceInfo.DeviceNo = null;
                transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
            }
            else
            {
                if (!Utilities.IsGuidDeviceId(deviceInfo.DeviceNo) || deviceInfo.InvalidDeviceId)
                {
                    invalidDevice = deviceCode;
                    deviceInfo.DeviceNo = null;
                    transBit.Set((int)TransStatus.InvalidDI, true);
                }
            }

            if (ContainsUnicodeCharacter(message.Account))
            {
                transBit.Set((int)TransStatus.InvalidLoginName, true);
            }

            var ipConverted = message.IP.Ip2Long();

            var result = new RawTransactionModel
            {
                DeviceCode = deviceInfo.DeviceNo,
                InvalidDevice = string.IsNullOrWhiteSpace(invalidDevice) ? null : invalidDevice,
                TransStatus = Core.Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = Convert.ToDateTime(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")),
                IP = message.IP,
                IPId = ipConverted.IpNumber,
                IsIPV6 = ipConverted.IsIpV6,
                PluginID = 0,
                URL = string.IsNullOrWhiteSpace(url) ? ConvertNA2Empty(SubStringUrl(deviceInfo.HRef)) : SubStringUrl(url),
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = string.IsNullOrWhiteSpace(userAgent) ? ConvertNA2Empty(StripUnicodeCharactersFromString(deviceInfo.UserAgent)) : StripUnicodeCharactersFromString(userAgent),
                FingerprintCode = deviceInfo.DeviceFPCode != "N/A" ? deviceInfo.DeviceFPCode : null,
                FingerprintMoreInfo = deviceInfo.DeviceFPCode != "N/A" ? EncodeCapturedData(deviceInfo) : null,
                TransKey = message.TransKey,
                FlaggedCode = flaggedCode,
                BotDetection = GetBotDetectionTrue(deviceInfo),
                BotComponent = GetBotComponent(deviceInfo, flaggedCode)
            };

            return result;
        }

        private static void LogMessage(MessageQueue message)
        {
            if (AppSettings.WriteLog)
            {
                var accounts = AppSettings.WriteAccount.Split(new[] { ',', ';' });

                if (accounts.Any(x => x.Equals(message.Account, StringComparison.OrdinalIgnoreCase)) || message.SubscriberId == 0)
                {
                    LogService.Info(JsonConvert.SerializeObject(message));
                }
            }
        }

        private static Flagged Browserless(string userAgent, string deviceNo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userAgent) && IsHeadless(userAgent))
                {
                    return Flagged.Headless;
                }

                if (string.IsNullOrWhiteSpace(deviceNo))
                {
                    return Flagged.EmptyDevice;
                }

                if (deviceNo == "N/A" || !Utilities.IsGuidDeviceId(deviceNo))
                {
                    return Flagged.InvalidDevice;
                }

                if (string.IsNullOrWhiteSpace(userAgent))
                {
                    return Flagged.EmptyUserAgent;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            return Flagged.Human;
        }

        private static bool IsHeadless(string userAgent)
        {
            if (userAgent.Contains("Headless")) return true;

            return false;
        }

        private static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        private static string ConvertNA2Empty(string data)
        {
            return data != "N/A" ? data : null;
        }

        private static string SubStringUrl(string url)
        {
            return url.IndexOf('?') != -1 ? url.Substring(0, url.IndexOf('?')) : url;
        }

        private static String StripUnicodeCharactersFromString(string inputValue)
        {
            return Regex.Replace(inputValue, @"[^\u0000-\u007F]", String.Empty);
        }

        private static string EncodeCapturedData(DeviceInfoModel deviceInfo)
        {
            return Cipher.Encrypt(string.Format("{0}#{1}#{2}#{3}#{4}",
                deviceInfo.Battery,
                deviceInfo.CapturedDate,
                deviceInfo.OSName,
                deviceInfo.TimeZone,
                deviceInfo.LocalIps));
        }

        private string GetBotDetectionTrue(DeviceInfoModel deviceInfo)
        {
            try
            {
                var detection = deviceInfo.BotD?.Detection;
                if (!string.IsNullOrEmpty(detection))
                {
                    JObject jObject = JObject.Parse(detection);
                    IEnumerable<string> filteredKeys = jObject.Properties()
                        .Where(p => p.Value.Value<bool>("bot"))
                        .Select(p => p.Name);

                    return string.Join(",", filteredKeys);
                }
            }
            catch { }

            return string.Empty;
        }

        private string GetBotComponent(DeviceInfoModel deviceInfo, string flaggedCode)
        {
            var storeBotComponent = ConfigurationManager.AppSettings["BotD-BotComponent-ForHuman"];
            if (string.IsNullOrEmpty(storeBotComponent) || storeBotComponent == "0")
            {
                if (flaggedCode != Flagged.Human.ToString())
                {
                    return deviceInfo.BotD?.Component ?? string.Empty;
                }
            }
            else
            {
                return deviceInfo.BotD?.Component ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
