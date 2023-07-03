namespace Fanex.NAP.DCS.RawTransaction.Service
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.Model.TransactionMonitoring;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Queue;
    using Fanex.Oneyed.Infrastructure.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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
        private EasyNetQConnector _rabbitMQConsume;
        private EasyNetQConnector _rabbitMQMonitor;
        private readonly ICapturingDataService _capturingDataService;
        private readonly List<RawTransactionModel> _transactionList;                

        public Runner()
        {
            InitializeComponent();

            _capturingDataService = new CapturingDataService();

            _transactionList = new List<RawTransactionModel>();                        

        }

        protected override void OnStart(string[] args)
        {
            try
            {

                ConsumeMessageBus();
                MonitorMessageBus();

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
            _rabbitMQConsume.Disposed();
            _rabbitMQMonitor.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        protected override void OnShutdown()
        {
            _rabbitMQConsume.Disposed();
            _rabbitMQMonitor.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        private void ConsumeMessageBus()
        {
            _rabbitMQConsume = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                                      AppSettings.RabbitMQ_VirtualHost,
                                      AppSettings.RabbitMQ_PrefetchCount,
                                      AppSettings.RabbitMQ_UserName,
                                      AppSettings.RabbitMQ_Password);

            _rabbitMQConsume.CreateBus()
                    .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                    .DeclareQueue(AppSettings.RabbitMQ_QueueName)
                    .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                    .Bind();
        }

        private void MonitorMessageBus()
        {
            _rabbitMQMonitor = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                                      AppSettings.RabbitMQ_VirtualHost,
                                      AppSettings.RabbitMQ_PrefetchCount,
                                      AppSettings.RabbitMQ_UserName,
                                      AppSettings.RabbitMQ_Password);

            _rabbitMQMonitor.CreateBus()
                    .DeclareExchange(AppSettings.RabbitMQ_Exchange_Monitor)
                    .DeclareQueue(AppSettings.RabbitMQ_QueueName_Monitor)
                    .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey_Monitor)
                    .Bind();
        }

        private void ConsumerTransactions()
        {
            if (ConfigurationManager.AppSettings["Queue-Consume"] == "1")
            {
                var trans = new List<RawTransactionModel>();
                var tranOfExtraSubs = new List<RawTransactionModel>();

                _rabbitMQConsume.AdvanceBus.Consume<MessageQueue>(_rabbitMQConsume.Queue, (message, info) =>
                {
                    try
                    {
                        var isTransOfExtraSubs = false;
                        var isTransOfDCSubs = true;
                        
                        var extraSubs = AppSettings.ExtraSubs;
                        var ignoreSubs = AppSettings.IgnoreSubs;

                        var messageBody = message.Body;

                        if (AppSettings.IsReadyDbExtra && !string.IsNullOrWhiteSpace(extraSubs))
                        {
                            messageBody.Subscriber = "velki";
                            isTransOfExtraSubs = extraSubs.Split(';').Any(x => x.Equals(messageBody.Subscriber, StringComparison.CurrentCultureIgnoreCase));
                        }
                        if (!string.IsNullOrWhiteSpace(ignoreSubs))
                        {
                            isTransOfDCSubs = !ignoreSubs.Split(';').Any(x => x.Equals(messageBody.Subscriber, StringComparison.CurrentCultureIgnoreCase));
                        }

                        var tran = TransformRawTransaction(messageBody);

                        if (String.IsNullOrEmpty(tran.LoginName))
                        {                          
                            LogService.Error("Test Exeption: Subscribers " + tran.SubscriberName + " sent account loginName is null. \nTrans info: " + JsonConvert.SerializeObject(tran));
                        }
                        else
                        {
                            if (isTransOfExtraSubs)
                            {
                                tranOfExtraSubs.Add(tran);

                                if (tranOfExtraSubs.Count >= AppSettings.TransOfExtraSubsBatch)
                                {
                                    InsertTransaction(tranOfExtraSubs, true);

                                    tranOfExtraSubs.Clear();
                                }
                            }
                            else if (isTransOfDCSubs)
                            {
                                trans.Add(tran);

                                if (trans.Count >= AppSettings.TransBatch)
                                {
                                    InsertTransaction(trans, false);

                                    PushTransactionToMonitor(trans);

                                    trans.Clear();
                                }
                            }
                        }

                        #region Delete
                        //else if (!string.IsNullOrWhiteSpace(ignoreSubs)) 
                        //{
                        //    var ignoreSubList = ignoreSubs.Split(';');

                        //    if (!ignoreSubList.Any(x => x.Equals(messageBody.Subscriber, StringComparison.CurrentCultureIgnoreCase)))
                        //    {
                        //        var tran = TransformRawTransaction(messageBody);

                        //        if (String.IsNullOrEmpty(tran.LoginName))
                        //        {
                        //            var transInfo = JsonConvert.SerializeObject(tran);
                        //            LogService.Error("Test Exeption: Subscribers " + tran.SubscriberName + " sent account loginName is null. \nTrans info: " + transInfo);
                        //        }
                        //        else
                        //        {
                        //            trans.Add(tran);

                        //            if (trans.Count >= AppSettings.TransBatch)
                        //            {
                        //                InsertTransaction(trans, false);

                        //                PushTransactionToMonitor(trans);

                        //                trans.Clear();
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        LogService.Error(logMessage);

                        trans.Clear();
                        tranOfExtraSubs.Clear();
                    }
                });
            }
        }

        private void InsertTransaction(List<RawTransactionModel> trans, bool isTransOfDBExtra)
        {
            try
            {
                _capturingDataService.InsertTransaction(trans, isTransOfDBExtra);                                
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);                                
            }
        }

        private void PushTransactionToMonitor(List<RawTransactionModel> trans)
        {
            if (ConfigurationManager.AppSettings["PushTrans-To-Monitor"] == "1")
            {
                var groupTrans = trans
                                .Select(x => { x.TransTime = Convert.ToDateTime(x.TransTime.ToString("MM/dd/yyyy")); return x; })
                                .GroupBy(x => new { x.SubscriberId, x.TransTime })
                                .Select(x => new TransactionMonitoringModel
                                {
                                    SubscriberID = x.Key.SubscriberId,                                    
                                    TransDate = x.Key.TransTime,
                                    TransTotal = x.Count()
                                });

                if (groupTrans != null && groupTrans.Any())
                {
                    groupTrans.ToList().ForEach(x =>
                    {
                        var message = new Message<TransactionMonitoringModel>(x, new MessageProperties() { DeliveryMode = 2, Priority = 9 });

                        _rabbitMQMonitor.AdvanceBus.Publish(_rabbitMQMonitor.Exchange, _rabbitMQMonitor.RoutingKey, true, message);
                    });
                }
            }
        }

        private RawTransactionModel TransformRawTransaction(MessageQueue message)
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
                var analysis = Utilities.GetAnalysis(message.Device);

                var browserless = Browserless(message.Token ?? string.Empty, analysis, userAgent, deviceInfo.DeviceNo);
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
                IPId = message.IP.Ip2Long().IpNumber,
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
                BotComponent = deviceInfo.BotD?.Component ?? string.Empty
            };

            return result;
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
            catch{}

            return string.Empty;
        }

        private string GetBotComponent(DeviceInfoModel deviceInfo, string flaggedCode)
        {
            if (flaggedCode != Flagged.Human.ToString())
            {
                return deviceInfo.BotD?.Component ?? string.Empty;
            }

            return string.Empty;
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

                if (accounts.Any(x => x.Equals(message.Account, StringComparison.OrdinalIgnoreCase)) || message.SubscriberId == 0)
                {
                    LogService.Info(JsonConvert.SerializeObject(message));
                }
            }           
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