namespace Fanex.NAP.DCS.RawTransaction.Service
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Queue;
    using Fanex.Oneyed.Infrastructure.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text.RegularExpressions;
    using System.Timers;
    using Utilities = Core.Utilities;

    public partial class Runner : ServiceBase
    {
        private readonly EasyNetQConnector _rabbitMQ;
        private readonly ICapturingDataService _capturingDataService;
        private readonly List<RawTransactionModel> _transactionList;
        private readonly System.Timers.Timer _timer;
        private DateTime _lastInsertTrans;

        public Runner()
        {
            InitializeComponent();

            _capturingDataService = new CapturingDataService();

            _rabbitMQ = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                                       AppSettings.RabbitMQ_VirtualHost,
                                       AppSettings.RabbitMQ_PrefetchCount,
                                       AppSettings.RabbitMQ_UserName,
                                       AppSettings.RabbitMQ_Password);
            _rabbitMQ.CreateBus()
                     .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                     .DeclareQueue(AppSettings.RabbitMQ_QueueName)
                     .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                     .Bind();

            _transactionList = new List<RawTransactionModel>();

            _timer = new System.Timers.Timer();
            _timer.Elapsed += InsertTransactionByTimer;
            _timer.Interval = AppSettings.Interval;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ConsumerTransactions();

                LogService.Info($"Raw trans service is STARTED at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        protected override void OnStop()
        {
            _rabbitMQ.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        protected override void OnShutdown()
        {
            _rabbitMQ.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        private void ConsumerTransactions()
        {
            if (ConfigurationManager.AppSettings["Queue-Consume"] == "1")
            {
                var trans = new List<RawTransactionModel>();                                

                _rabbitMQ.AdvanceBus.Consume<MessageQueue>(_rabbitMQ.Queue, (message, info) =>
                {
                    try
                    {
                        var messageBody = message.Body;

                        var ignoreSubs = ConfigurationManager.AppSettings["Ignore-Subs"];

                        if (!string.IsNullOrWhiteSpace(ignoreSubs))
                        {
                            var ignoreSubList = ignoreSubs.Split(';');

                            if (!ignoreSubList.Any(x => x.Equals(messageBody.Subscriber, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                var tran = TransformRawTransaction(messageBody);

                                trans.Add(tran);

                                if (trans.Count >= AppSettings.TransBatch)
                                {
                                    InsertTransaction(trans);

                                    trans.Clear();

                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        LogService.Error(logMessage);

                        trans.Clear();
                    }
                });
            }
        }

        private void InsertTransaction(List<RawTransactionModel> trans)
        {
            try
            {
                _capturingDataService.InsertTransaction(trans);

                _lastInsertTrans = DateTime.Now;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                _lastInsertTrans = DateTime.Now;
            }
        }

        private RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            LogMessage(message);
                        
            var invalidDevice = string.Empty;
            var deviceCode = Utilities.GetDeviceCodeFromCapturedData(message.Device);
            var deviceInfo = _capturingDataService.GetDeviceInfo(deviceCode);
            var deviceNo = deviceInfo.DeviceNo;
            var url = Utilities.GetURLFromDeviceCode(message.Device);
            var userAgent = Utilities.GetUserAgentFromDeviceCode(message.Device);
            var transBit = new BitArray(16);
            var token = message.Token ?? string.Empty;
            var analysis = Utilities.GetAnalysis(message.Device);

            var flagged = Browserless(token, analysis, userAgent, deviceNo);

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

            var rawFlagged = _botTransDetection.IsBotTransaction(message);
            var flagged = Enum.IsDefined(typeof(DeviceFlagged), rawFlagged);
            var result = new RawTransactionModel
            {
                DeviceCode = deviceInfo.DeviceNo,
                InvalidDevice = string.IsNullOrWhiteSpace(invalidDevice) ? null : invalidDevice,
                TransStatus = Core.Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                TransTime = Convert.ToDateTime(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")),
                IP = message.IP,
                IPId = message.IP.Ip2Long().IpNumber,
                PluginID = 0,
                Flagged = (int)flagged,
                URL = string.IsNullOrWhiteSpace(url) ? ConvertNA2Empty(SubStringUrl(deviceInfo.HRef)) : SubStringUrl(url),
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = string.IsNullOrWhiteSpace(userAgent) ? ConvertNA2Empty(StripUnicodeCharactersFromString(deviceInfo.UserAgent)) : StripUnicodeCharactersFromString(userAgent),
                FingerprintCode = deviceInfo.DeviceFPCode != "N/A" ? deviceInfo.DeviceFPCode : null,
                FingerprintMoreInfo = deviceInfo.DeviceFPCode != "N/A" ? EncodeCapturedData(deviceInfo) : null
            };                       

            return result;
        }

        private void InsertTransactionByTimer(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _lastInsertTrans).Minutes >= 1 && _transactionList.Count != 0)
            {
                InsertTransaction(_transactionList);

                _transactionList.Clear();
            }
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
        private static string SubStringUrl(string url)
        {
            return url.IndexOf('?') != -1 ? url.Substring(0, url.IndexOf('?')) : url;
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

        private static void LogMessage(MessageQueue message)
        {
            if (AppSettings.WriteLog)
            {
                var accounts = AppSettings.WriteAccount.Split(new[] { ',', ';' });

                if (accounts.Any(x => x.Equals(message.Account, StringComparison.OrdinalIgnoreCase)))
                {
                    LogService.Info(JsonConvert.SerializeObject(message));
                }
            }

        }
        private static Flagged Browserless(string token, string analysis, string userAgent, string deviceNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deviceNo))
                {
                    return Flagged.EmptyDevice;
                }
                else
                {
                    if (deviceNo != "N/A" && Utilities.IsGuidDeviceId(deviceNo))
                    {
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            return Flagged.EmptyToken;
                        }
                        else
                        {
                            if (!IsValidateToken(token))
                            {
                                return Flagged.InvalidToken;
                            }
                        }

                        //if (string.IsNullOrWhiteSpace(analysis))
                        //{
                        //    return Flagged.EmptyAnalysis;
                        //}
                        //else
                        //{
                        //    if (!IsValidAnalysisKey(analysis))
                        //    {
                        //        return Flagged.InvalidAnalysis;
                        //    }
                        //}

                        if (string.IsNullOrWhiteSpace(userAgent))
                        {
                            return Flagged.EmptyUserAgent;
                        }
                        else
                        {
                            if (IsHeadless(userAgent))
                            {
                                return Flagged.Headless;
                            }
                        }
                    }
                    else
                    {
                        return Flagged.InvalidDevice;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }

            return Flagged.Human;
        }


        private static bool IsValidateToken(string token)
        {
            var decryptedToken = BrowserlessDetection.Decrypt(token);
            var splitedToken = decryptedToken.Split('@');
            if (splitedToken.Length != 2)
            {
                return false;
            }
            else
            {
                if (splitedToken.Length > 0)
                {
                    var submitTime = DateTime.Parse(splitedToken[0]);

                    if (DateTime.Now.ToUniversalTime() < submitTime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsValidAnalysisKey(string analysis)
        {
            string JsEnabledKey = "js-enabled";
            string[] AnalysisFormatKey = new string[] { "startTime", "version", "exceptions", "executions", "storages", "devices", "enable" };

            if (!analysis.Contains(JsEnabledKey))
            {
                foreach (var analysiItem in AnalysisFormatKey)
                {
                    if (!analysis.Contains(analysiItem))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsHeadless(string userAgent)
        {
            if (userAgent.Contains("Headless")) return true;

            return false;
        }
    }
}